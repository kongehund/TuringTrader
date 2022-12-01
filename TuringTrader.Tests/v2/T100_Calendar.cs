﻿//==============================================================================
// Project:     TuringTrader: SimulatorEngine.Tests
// Name:        T100_Calendar
// Description: Unit test for trading calendar.
// History:     2022xi30, FUB, created
//------------------------------------------------------------------------------
// Copyright:   (c) 2011-2022, Bertram Enterprises LLC
// License:     This file is part of TuringTrader, an open-source backtesting
//              engine/ market simulator.
//              TuringTrader is free software: you can redistribute it and/or 
//              modify it under the terms of the GNU Affero General Public 
//              License as published by the Free Software Foundation, either 
//              version 3 of the License, or (at your option) any later version.
//              TuringTrader is distributed in the hope that it will be useful,
//              but WITHOUT ANY WARRANTY; without even the implied warranty of
//              MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//              GNU Affero General Public License for more details.
//              You should have received a copy of the GNU Affero General Public
//              License along with TuringTrader. If not, see 
//              https://www.gnu.org/licenses/agpl-3.0.
//==============================================================================

#region libraries
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TuringTrader.SimulatorV2.Tests
{
    [TestClass]
    public class T100_Calendar
    {
        private class Testbed : Algorithm
        {
            public List<DateTime> TestResult;
            public override void Run()
            {
                StartDate = DateTime.Parse("2020-01-01T16:00-05:00");
                EndDate = DateTime.Parse("2020-12-31T16:00-05:00");
                WarmupPeriod = TimeSpan.FromDays(0);

                TestResult = TradingCalendar.TradingDays;
            }
        }

        [TestMethod]
        public void Test_DataRetrieval()
        {
            var algo = new Testbed();
            algo.Run();
            var result = algo.TestResult;

            var firstDate = result.First();
            Assert.IsTrue(firstDate == DateTime.Parse("2020-01-02T16:00-5:00"));

            var lastDate = result.Last();
            Assert.IsTrue(lastDate == DateTime.Parse("2020-12-31T16:00-5:00"));

            var barCount = result.Count();
            Assert.IsTrue(barCount == 253);
        }
    }
}

//==============================================================================
// end of file