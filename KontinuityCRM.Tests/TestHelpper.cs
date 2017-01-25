using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace KontinuityCRM.Tests
{
    public class TestHelpper
    {
        public static IMappingEngine CreateMapperEngine()
        {
            DtoMapperConfig.CreateMaps();
            return Mapper.Engine;
        }
    }
}
