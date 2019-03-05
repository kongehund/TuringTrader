﻿//==============================================================================
// Project:     TuringTrader, simulator core
// Name:        LogEntry
// Description: log entry
// History:     2018ix11, FUB, created
//------------------------------------------------------------------------------
// Copyright:   (c) 2017-2018, Bertram Solutions LLC
//              http://www.bertram.solutions
// License:     This code is licensed under the term of the
//              GNU Affero General Public License as published by 
//              the Free Software Foundation, either version 3 of 
//              the License, or (at your option) any later version.
//              see: https://www.gnu.org/licenses/agpl-3.0.en.html
//==============================================================================

#region libraries
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace TuringTrader.Simulator
{

    /// <summary>
    /// Entry to trading log.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Symbol traded. This is required, as the Instrument is cleared from
        /// the OrderTicket field to preserve memory.
        /// </summary>
        public string Symbol;

        /// <summary>
        /// Original order ticket. Please note that the Instrument is set to
        /// null, to preserve memory. Use the Symbol field instead.
        /// </summary>
        public Order OrderTicket;

        /// <summary>
        /// Bar of trade execution.
        /// </summary>
        public Bar BarOfExecution;

        /// <summary>
        /// Net asset value at trade execution.
        /// </summary>
        public double NetAssetValue;

        /// <summary>
        /// Fill price of trade.
        /// </summary>
        public double FillPrice;

        /// <summary>
        /// Commission paid for trade.
        /// </summary>
        public double Commission;

        /// <summary>
        /// Return string with order action. This is for convenience only,
        /// as this information can be reconstructed from the other fields.
        /// </summary>
        public LogEntryAction Action
        {
            get
            {
                switch(OrderTicket.Type)
                {
                    case OrderType.cash:
                        if (OrderTicket.Quantity > 0) return LogEntryAction.Withdrawal;
                        else return LogEntryAction.Deposit;

                    case OrderType.optionExpiryClose:
                    case OrderType.stockInactiveClose:
                        return LogEntryAction.Expiry;

                    default:
                        if (OrderTicket.Quantity > 0) return LogEntryAction.Buy;
                        else return LogEntryAction.Sell;
                }
            }
        }

        /// <summary>
        /// Instrument class. This field is required, as the Instrument
        /// is cleared from the OrderTicket to save memory.
        /// </summary>
        public LogEntryInstrument InstrumentType;
    }
}

//==============================================================================
// end of file