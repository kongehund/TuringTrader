﻿//==============================================================================
// Project:     TuringTrader, demo algorithms
// Name:        Demo07_Subclassing
// Description: demonstrate subclassable algorithms
// History:     2019v21, FUB, created
//              2023iii02, FUB, updated for v2 engine
//------------------------------------------------------------------------------
// Copyright:   (c) 2011-2023, Bertram Enterprises LLC dba TuringTrader.
//              https://www.turingtrader.org
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringTrader.SimulatorV2;
using TuringTrader.SimulatorV2.Indicators;
using TuringTrader.SimulatorV2.Assets;
#endregion

namespace Demos
{
    // child algorithm implementing 60/40 portfolio
    // note that this algorithm is not public. if it was,
    // we would not be able to run this demo directly from
    // its C# source code
    class ChildSixtyFourty : Algorithm
    {
        public override void Run()
        {
            //---------- initialization

            // set the simulation period
            // note how we keep StartDate and EndDate intact, 
            // if they are alreay set. both values come pre-populated
            // when the parent algorithm calls us
            StartDate = StartDate ?? DateTime.Parse("2007-01-01T16:00-05:00");
            EndDate = EndDate ?? DateTime.Parse("2022-12-31T16:00-05:00");
            WarmupPeriod = TimeSpan.FromDays(5);

            //---------- simulation

            SimLoop(() =>
            {
                // rebalance monthly
                if (IsFirstBar || SimDate.Month != NextSimDate.Month)
                {
                    Asset(ETF.SPY).Allocate(0.6, OrderType.openNextBar);
                    Asset(ETF.AGG).Allocate(0.4, OrderType.openNextBar);

                    // as a default, the parent algorithm receives
                    // bars with the child strategy's NAV
                    // alternatively, we could also return OHLCV bars here
                }
            });
        }
    }

    public class Demo07_ChildAlgos : Algorithm
    {
        public override void Run()
        {
            //---------- initialization

            // set the simulation period
            StartDate = DateTime.Parse("2007-01-01T16:00-05:00");
            EndDate = DateTime.Parse("2022-12-31T16:00-05:00");

            // instantiate our child algo
            // both variants work
#if true
            // caution: we must make sure to use the same
            // instance for all calls. Therefore, we need
            // to instantiate the algo outside of SimLoop
            var childAlgo = new ChildSixtyFourty();
#else
            // when using the string variant, there are
            // no such restrictions
            var childAlgo = "algorithm:ChildSixtyFourty"
#endif

            //---------- simulation
                
            SimLoop(() =>
            {
                // we put all our capital into the child algo
                // note that on this level, a child algo is
                // indistinguishable from any other asset
                if (IsFirstBar)
                    Asset(childAlgo).Allocate(1.0, OrderType.openNextBar);

                Plotter.SelectChart("child algorithms", "date");
                Plotter.SetX(SimDate);
                Plotter.Plot("nav", NetAssetValue);
                Plotter.Plot("spy", Asset(ETF.SPY).Close[0]);
                Plotter.Plot("agg", Asset(ETF.AGG).Close[0]);
            });

            //---------- post-processing

            // the target allocation and the historical allocation
            // will show the assets held by the child strategy
            Plotter.AddTargetAllocation();
            Plotter.AddHistoricalAllocations();

            // in contrast to that, the trade log will only
            // show the single trade made by the parent algorithm
            Plotter.AddTradeLog();
        }
    }
}

//==============================================================================
// end of file