﻿//==============================================================================
// Project:     TuringTrader, simulator core v2
// Name:        IAccount
// Description: Trading account interface.
// History:     2023ii10, FUB, created
//------------------------------------------------------------------------------
// Copyright:   (c) 2011-2023, Bertram Enterprises LLC dba TuringTrader.
//              https://www.turingtrader.org
// License:     This file is part of TuringTrader, an open-source backtesting
//              engine/ trading simulator.
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

using System;
using System.Collections.Generic;

namespace TuringTrader.SimulatorV2
{
    /// <summary>
    /// OrderType is an enumeration of orders supported by the
    /// simulator.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// buy or sell assets on this bar's close
        /// </summary>
        closeThisBar,
        /// <summary>
        /// buy or sell assets on the next bar's open
        /// </summary>
        openNextBar,
        // SellStopNextBar,
        // BuyLimitNextBar,
    }

    /// <summary>
    /// Trading calendar class to convert a date range to
    /// an enumerable of valid trading days.
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Submit and queue order.
        /// </summary>
        /// <param name="Name">asset name</param>
        /// <param name="weight">asset target allocation</param>
        /// <param name="orderType">order type</param>
        public void SubmitOrder(string Name, double weight, OrderType orderType);

        /// <summary>
        /// Process bar. This method will loop through the queued
        /// orders, execute them as required, and return a bar
        /// representing the strategy's NAV.
        /// </summary>
        /// <returns></returns>
        public OHLCV ProcessBar();

        /// <summary>
        /// Return net asset value in currency, starting with $1,000
        /// at the beginning of the simulation. Note that currency
        /// has no relevance throughout the v2 engine. We use this
        /// value to make the NAV more tangible during analysis and
        /// debugging.
        /// </summary>
        public double NetAssetValue { get; }

        /// <summary>
        /// Return positions, as fraction of NAV.
        /// </summary>
        public Dictionary<string, double> Positions { get; }

        /// <summary>
        /// Return of cash available, as fraction of NAV.
        /// </summary>
        public double Cash { get; }

        /// <summary>
        /// Container collecting all order information at time of order submittal.
        /// </summary>
        public class OrderTicket
        {
            /// <summary>
            /// Asset name. This is the name that was used to load the asset,
            /// which may or may not be identical to the asset's ticker symbol.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Asset target allocation, as fraction of NAV.
            /// </summary>
            public readonly double TargetAllocation;

            /// <summary>
            /// Order type.
            /// </summary>
            public readonly OrderType OrderType;

            /// <summary>
            /// Order submit date.
            /// </summary>
            public readonly DateTime SubmitDate;

            /// <summary>
            /// Create new order ticket.
            /// </summary>
            /// <param name="symbol"></param>
            /// <param name="targetAllocation"></param>
            /// <param name="orderType"></param>
            /// <param name="submitDate"></param>
            public OrderTicket(string symbol, double targetAllocation, OrderType orderType, DateTime submitDate)
            {
                Name = symbol;
                TargetAllocation = targetAllocation;
                OrderType = orderType;
                SubmitDate = submitDate;
            }
        }

        /// <summary>
        /// Container collecting all order information at time of order execution.
        /// </summary>
        public class OrderReceipt
        {
            /// <summary>
            /// Order ticket.
            /// </summary>
            public readonly OrderTicket OrderTicket;

            /// <summary>
            /// Order execution date.
            /// </summary>
            public readonly DateTime ExecDate;

            /// <summary>
            /// Order size, as a fraction of NAV.
            /// </summary>
            public readonly double OrderSize;

            /// <summary>
            /// Order fill price.
            /// </summary>
            public readonly double FillPrice;

            /// <summary>
            /// Currency spent/received for assets traded. Note that throughout
            /// the v2 engine, currency has no significance. This is only to
            /// make trades more tangible while analyzing and debugging.
            /// </summary>
            public readonly double OrderAmount;

            /// <summary>
            /// Currency lost for trade friction. Note that throughout the v2
            /// engine, currency has no significance. This is only to make trades
            /// more tangible while analyzing and debugging.
            /// </summary>
            public readonly double FrictionAmount;

            /// <summary>
            /// Create order receipt.
            /// </summary>
            /// <param name="orderTicket"></param>
            /// <param name="orderSize"></param>
            /// <param name="fillPrice"></param>
            /// <param name="orderAmount"></param>
            /// <param name="frictionAmount"></param>
            public OrderReceipt(OrderTicket orderTicket,
                DateTime execDate,
                double orderSize,
                double fillPrice,
                double orderAmount,
                double frictionAmount)
            {
                OrderTicket = orderTicket;
                ExecDate = execDate;
                OrderSize = orderSize;
                FillPrice = fillPrice;
                OrderAmount = orderAmount;
                FrictionAmount = frictionAmount;
            }
        }

        /// <summary>
        /// Retrieve trade log. Note that this log only contains trades executed
        /// and not orders that were not executed.
        /// </summary>
        public List<OrderReceipt> TradeLog { get; }
    }
}

//==============================================================================
// end of file