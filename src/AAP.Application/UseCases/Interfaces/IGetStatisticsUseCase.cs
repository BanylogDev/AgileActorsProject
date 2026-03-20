using AAP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AAP.Application.UseCases.Interfaces
{
    public interface IGetStatisticsUseCase
    {
        List<Statistics> Execute();
    }
}
