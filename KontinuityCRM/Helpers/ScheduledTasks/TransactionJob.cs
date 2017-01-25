using AutoMapper;
using KontinuityCRM.Models;
using KontinuityCRM.Models.Gateways;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KontinuityCRM.Helpers.ScheduledTasks
{
    [PersistJobDataAfterExecution]
    public class TransactionJob : IJob
    {
        private readonly IUnitOfWork uow;
        private readonly IMappingEngine mapper;

        public TransactionJob(IUnitOfWork uow, IMappingEngine mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        //public CaptureJob() : this(new UnitOfWork(), Mapper.Engine) { }

        /// <summary>
        /// Execute Capture Job
        /// </summary>
        /// <param name="context">job execution context</param>
        public async void Execute(IJobExecutionContext context)
        {
            context.JobDetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow;

            var transactionQueueMasterData = uow.TransactionQueueMasterRepository.Get().Where(a => a.finished == false).ToList();
            foreach (var transactionQueMasterDataValues in transactionQueueMasterData)
            {
                // get all schedule capture and make the transaction
                var transactionList = uow.TransactionQueueRepository.Get().Where(a => a.finished == false && a.PrepaidCard.declined == false && a.PrepaidCard.Stop == false && a.TransactionQueMasterId== transactionQueMasterDataValues.Id).ToList(); // false if null
                foreach (var transactions in transactionList)
                {
                    ///  bool transactionQueueTableUpdate = false, prepaidCardTableUpdate = false, transactionViaPrepaidCardSave = false, transactionQueueMasterUpdate = false;

                    //   var transactionQueueBeforeException = transactions;
                    //  TransactionViaPrepaidCardQueue transactionViaPrepaidCardBeforeException =new TransactionViaPrepaidCardQueue();
                    //  TransactionQueueMaster transactionQueueMasterBeforeException = new TransactionQueueMaster();
                    try
                    {
                        TransactionQueueMaster transactionMasterBefore = uow.TransactionQueueMasterRepository.Get().Where(a => a.Id == transactions.TransactionQueMasterId).FirstOrDefault();
                        //  transactionQueueMasterBeforeException = transactionMaster;
                        int remainingTrans = uow.TransactionViaPrepaidCardQueueRepository.Get().Where(a => a.TransactionQueueMasterId == transactionQueMasterDataValues.Id && a.Success == true).ToList().Count;
                        transactionMasterBefore.RemainingTransactions = transactionMasterBefore.NoOfTransactions - remainingTrans;
                        if (transactionMasterBefore.RemainingTransactions == 0)
                        {
                            transactionMasterBefore.finished = true;
                        }
                        uow.TransactionQueueMasterRepository.Update(transactionMasterBefore);
                        uow.Save();
                        if (transactions != null && transactionMasterBefore.finished==false)
                        {
                            transactions.Attempt = transactions.Attempt + 1;
                            uow.TransactionQueueRepository.Update(transactions);
                            uow.Save();
                            //  transactionQueueTableUpdate = true;

                            Random generator = new Random();
                            String r = generator.Next(transactions.TimeIntervalMin, transactions.TimeIntervalMax).ToString("D5");
                            System.Threading.Thread.Sleep(Convert.ToInt32(r));

                            if (transactions.PrepaidCard.Stop == false && transactions.PrepaidCard.declined == false && Convert.ToDecimal(transactions.PrepaidCard.RemainingAmount) > 0)
                            {
                                GatewayModel gatewayModel = null;
                                gatewayModel = transactions.Processor.GatewayModel(mapper);
                                TransactionViaPrepaidCardQueue tran = gatewayModel.SalePrepaidCard(transactions.PrepaidCard, transactions.Processor, transactions.Amount);
                                //   transactionViaPrepaidCardBeforeException = tran;
                                if (tran.Success == true)
                                {
                                    //update transaction que to finished after success. Increase Attempt 
                                    transactions.finished = true;
                                    transactions.LastUpdatedDate = DateTime.UtcNow;
                                    uow.TransactionQueueRepository.Update(transactions);
                                    uow.Save();
                                    //   transactionQueueTableUpdate = true;


                                    //update prepaid card if stoped or declined. Increase Attempt 
                                    transactions.PrepaidCard.Date = DateTime.UtcNow;
                                    transactions.PrepaidCard.RemainingAmount = (Convert.ToDecimal(transactions.PrepaidCard.RemainingAmount) - Convert.ToDecimal(transactions.Amount)).ToString();
                                    if (Convert.ToDecimal(transactions.PrepaidCard.RemainingAmount) <= 0)
                                    {
                                        transactions.PrepaidCard.Stop = true;
                                    }
                                    uow.PrepaidCardRepository.Update(transactions.PrepaidCard);
                                    uow.Save();
                                    //  prepaidCardTableUpdate = true;
                                }
                                else if(Convert.ToDecimal(transactions.PrepaidCard.RemainingAmount) <= 0)
                                {
                                    // will update 
                                    transactions.PrepaidCard.Date = DateTime.UtcNow;
                                    transactions.PrepaidCard.RemainingAmount = "0";
                                    transactions.PrepaidCard.Stop = true;
                                   
                                    uow.PrepaidCardRepository.Update(transactions.PrepaidCard);
                                    uow.Save();
                                }

                                if (tran.Status == Models.TransactionStatus.Declined)
                                {
                                    transactions.LastUpdatedDate = DateTime.UtcNow;
                                    uow.TransactionQueueRepository.Update(transactions);
                                    uow.Save();
                                    //  transactionQueueTableUpdate = true;


                                    transactions.PrepaidCard.Date = DateTime.UtcNow;
                                    transactions.PrepaidCard.declined = true;
                                    uow.PrepaidCardRepository.Update(transactions.PrepaidCard);
                                    uow.Save();
                                    //  prepaidCardTableUpdate = true;
                                }
                                tran.TransactionQueueMasterId = transactionQueMasterDataValues.Id;
                                uow.TransactionViaPrepaidCardQueueRepository.Add(tran);
                                uow.Save();
                                //  transactionViaPrepaidCardSave = true;
                            }
                            TransactionQueueMaster transactionMaster = uow.TransactionQueueMasterRepository.Get().Where(a => a.Id == transactions.TransactionQueMasterId).FirstOrDefault();
                            //  transactionQueueMasterBeforeException = transactionMaster;
                           remainingTrans = uow.TransactionViaPrepaidCardQueueRepository.Get().Where(a => a.TransactionQueueMasterId == transactionQueMasterDataValues.Id && a.Success==true).ToList().Count;
                           
                            transactionMaster.RemainingTransactions = transactionMaster.NoOfTransactions - remainingTrans;
                            if (transactionMaster.RemainingTransactions == 0)
                            {
                                transactionMaster.finished = true;
                            }
                            uow.TransactionQueueMasterRepository.Update(transactionMaster);
                            uow.Save();
                            // transactionQueueMasterUpdate = true;
                        }
                    }
                    catch (Exception ex)
                    {


                        //failed saving data skipped the transaction
                    }
                }
            }
        }
    }
}