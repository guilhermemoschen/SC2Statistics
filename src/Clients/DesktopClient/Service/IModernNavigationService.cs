// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModernNavigationService.cs" company="saramgsilva">
//   Copyright (c) 2014 saramgsilva. All rights reserved.
// </copyright>
// <summary>
//   The ModernNavigationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Views;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    /// <summary>
    /// The ModernNavigationService interface.
    /// </summary>
    public interface IModernNavigationService : INavigationService
    {
        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        object Parameter { get; }

        Uri GetPageUri(string pageKey);
    }
}
