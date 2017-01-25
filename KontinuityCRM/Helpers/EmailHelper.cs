using AutoMapper;
using KontinuityCRM.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Quartz.Job;
using Quartz.Impl;
using KontinuityCRM.Helpers.ScheduledTasks;

namespace KontinuityCRM.Helpers
{

    public class EmailHelper
    {
        private readonly IScheduler _Scheduler;
        public static void SendOrderEmail(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId)
        {
            var order = uow.OrderRepository.Find(orderId);
            Processor processor = null; // processor that will be used
            BalancerProcessor bp = null; // balancer processor if used
            int? balancerId = null; // balancer if there is a balancerprocessor

            order.FindProcessor(uow, out processor, out bp, out balancerId);

            var gatewayModel = processor.GatewayModel(mapper);
           
            // TODO
            // Retarded logic - this needs to populate product specific values, not send an email for every product
            var orderProduct = order.OrderProducts.ToList();
            var product = orderProduct.FirstOrDefault().Product;
            var Events = product.ProductEvents.FirstOrDefault(p => p.Event.Type == notificationType);
            if (Events != null)
            {
                var _event = Events.Event;
                var sbhtmlbody = KontinuityCRMHelper.PopulateTokens(_event.Template.HtmlBody, order, orderProduct, processor, bp, gatewayModel, balancerId);
                var sbbody = KontinuityCRMHelper.PopulateTokens(_event.Template.TextBody, order, orderProduct, processor, bp, gatewayModel, balancerId);

                // refactored all token replacement to KontinuityCRMHelper.PopulateTokens 
                KontinuityCRMHelper.SendMail(
                   MailServer: _event.SmtpServer.Host,
                   MailPort: _event.SmtpServer.Port,
                   MailUsername: _event.SmtpServer.UserName,
                   MailPassword: _event.SmtpServer.Password,
                   MailEnableSsl: _event.SmtpServer.Authorization,
                   from: _event.SmtpServer.Email,
                   to: order.Email,
                   subject: _event.Template.Subject,
                   textBody: sbbody.ToString(),
                   htmlBody: sbhtmlbody.ToString()
                   //body: _event.Template.HtmlBody,
                   //isBodyHtml: true
                   );

                TriggerEmailTable _triggerTable = new TriggerEmailTable();
                _triggerTable.EventId = _event.Id;
                _triggerTable.OrderId = order.OrderId;
                _triggerTable.TriggerTime = DateTime.UtcNow;
                _triggerTable.Type = notificationType;
                _triggerTable.sent = false;
                
                uow.TriggerEmailTableRepository.Add(_triggerTable);
                uow.Save();

            }
        }

        public static void SendSecondEmailTemplate(IUnitOfWork uow, IMappingEngine mapper, int orderId,int EventId)
        {
            var order = uow.OrderRepository.Find(orderId);
            Processor processor = null; // processor that will be used
            BalancerProcessor bp = null; // balancer processor if used
            int? balancerId = null; // balancer if there is a balancerprocessor

            order.FindProcessor(uow, out processor, out bp, out balancerId);

            var gatewayModel = processor.GatewayModel(mapper);

            // TODO
            // Retarded logic - this needs to populate product specific values, not send an email for every product
            var orderProduct = order.OrderProducts.ToList();
            var product = orderProduct.FirstOrDefault().Product;
            var events = uow.EventRepository.Get().Where(a => a.Id == EventId).FirstOrDefault();
            if (events != null)
            {
                string templatebody = uow.EmailTemplateRepository.Get().Where(a => a.Id == events.SecondTemplateId).FirstOrDefault().HtmlBody;
                var _event = events;
                var sbhtmlbody = KontinuityCRMHelper.PopulateTokens(templatebody, order, orderProduct, processor, bp, gatewayModel, balancerId);
                var sbbody = KontinuityCRMHelper.PopulateTokens(templatebody, order, orderProduct, processor, bp, gatewayModel, balancerId);

                // refactored all token replacement to KontinuityCRMHelper.PopulateTokens 
                KontinuityCRMHelper.SendMail(
                   MailServer: _event.SmtpServer.Host,
                   MailPort: _event.SmtpServer.Port,
                   MailUsername: _event.SmtpServer.UserName,
                   MailPassword: _event.SmtpServer.Password,
                   MailEnableSsl: _event.SmtpServer.Authorization,
                   from: _event.SmtpServer.Email,
                   to: order.Email,
                   subject: _event.Template.Subject,
                   textBody: sbbody.ToString(),
                   htmlBody: sbhtmlbody.ToString()
                   //body: _event.Template.HtmlBody,
                   //isBodyHtml: true
                   );

                //TriggerEmailTable _triggerTable = new TriggerEmailTable();
                //_triggerTable.EventId = _event.Id;
                //_triggerTable.OrderId = order.OrderId;
                //_triggerTable.TriggerTime = DateTime.UtcNow;
                //_triggerTable.Type = notificationType;

                //uow.TriggerEmailTableRepository.Add(_triggerTable);
                //uow.Save();

            }
        }



        //public static void SendOrderEmailNew(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId)
        //{
        //    var order = uow.OrderRepository.Find(orderId);
        //    Processor processor = null; // processor that will be used
        //    BalancerProcessor bp = null; // balancer processor if used
        //    int? balancerId = null; // balancer if there is a balancerprocessor

        //    order.FindProcessor(uow, out processor, out bp, out balancerId);

        //    var gatewayModel = processor.GatewayModel(mapper);

        //    // TODO
        //    // Retarded logic - this needs to populate product specific values, not send an email for every product
        //    var orderProduct = order.OrderProducts.ToList();
        //    var product = orderProduct.FirstOrDefault().Product;
        //    var Events = product.ProductEvents.FirstOrDefault(p => p.Event.Type == notificationType);
        //    if (Events != null)
        //    {
        //        var _event = Events.Event;
        //        var sbhtmlbody = KontinuityCRMHelper.PopulateTokens(_event.Template.HtmlBody, order, orderProduct, processor, bp, gatewayModel, balancerId);
        //        var sbbody = KontinuityCRMHelper.PopulateTokens(_event.Template.TextBody, order, orderProduct, processor, bp, gatewayModel, balancerId);

        //        // refactored all token replacement to KontinuityCRMHelper.PopulateTokens 
        //        KontinuityCRMHelper.SendMail(
        //           MailServer: _event.SmtpServer.Host,
        //           MailPort: _event.SmtpServer.Port,
        //           MailUsername: _event.SmtpServer.UserName,
        //           MailPassword: _event.SmtpServer.Password,
        //           MailEnableSsl: _event.SmtpServer.Authorization,
        //           from: _event.SmtpServer.Email,
        //           to: order.Email,
        //           subject: _event.Template.Subject,
        //           textBody: sbbody.ToString(),
        //           htmlBody: sbhtmlbody.ToString()
        //           //body: _event.Template.HtmlBody,
        //           //isBodyHtml: true
        //           );

        //        if (Events.Event.NoOfDays != null)
        //        {
        //            IJobDetail emailjon = new JobDetailImpl("emailJob", null, typeof(EmailJob));
        //            var hourlyTrigger1 = TriggerBuilder.Create()
        //        .StartNow()
        //        .WithSimpleSchedule
        //          (s => s
        //              .WithIntervalInHours(1)
        //              //.WithIntervalInSeconds(10)
        //              .RepeatForever())
        //        .Build();
        //        }
        //    }
        //}

        public static void SendForgotPasswordEmail(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, string Email, string forgotPasswordLink, INLogger logger)
        {
            logger.LogInfo("Step1:process started sending email at emailaddress:" + Email);
            var _smtpServer = uow.SmtpServerRepository.Get().FirstOrDefault();

            var _emailTemplate = uow.EmailTemplateRepository.Get(p => p.Name == "Forgot Password").FirstOrDefault();
            if (_emailTemplate != null && _smtpServer != null)
            {
                logger.LogInfo("Step2:Got smtp server, HOST:" + _smtpServer.Host + ", USERNAME:" + _smtpServer.UserName + ", PORT:" + _smtpServer.Port);
                var emailBody = KontinuityCRMHelper.PopulateTokens(_emailTemplate.HtmlBody, forgotPasswordLink);
                KontinuityCRMHelper.SendMail(
                   MailServer: _smtpServer.Host,
                   MailPort: _smtpServer.Port,
                   MailUsername: _smtpServer.UserName,
                   MailPassword: _smtpServer.Password,
                   MailEnableSsl: _smtpServer.Authorization,
                   from: _smtpServer.Email,
                   to: Email,
                   subject: _emailTemplate.Subject,
                   textBody: emailBody.ToString(),
                   htmlBody: emailBody.ToString()
                   //body: _event.Template.HtmlBody,
                   //isBodyHtml: true
                   );
                logger.LogInfo("Step3: Email sent successfully");
            }
        }
    }
}
