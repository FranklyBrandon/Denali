using Denali.Models.Shared;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Denali.Strategies
{
    public interface IAggregateStrategy
    {
        void Initialize(IEnumerable<IAggregateData> aggregateData);
        bool ProcessTick(IEnumerable<IAggregateData> aggregateData);
    }
}
