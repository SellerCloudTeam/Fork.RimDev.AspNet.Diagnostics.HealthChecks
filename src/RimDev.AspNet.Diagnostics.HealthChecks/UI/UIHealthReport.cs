﻿// Adapted from https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/tree/2.2.0-upgrade-ui-client-2.2.3/src/HealthChecks.UI.Client
// Originally licensed under the Apache License, Version 2.0, https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/2.2.0-upgrade-ui-client-2.2.3/LICENSE

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RimDev.AspNet.Diagnostics.HealthChecks.UI
{
    /*
     * Models for UI Client. This models represent a indirection between HealthChecks API and 
     * UI Client in order to implement some features not present on HealthChecks of substitute 
     * some properties etc.
     */
    public class UIHealthReport
    {
        public UIHealthStatus Status { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public Dictionary<string, UIHealthReportEntry> Entries { get; }

        public UIHealthReport(Dictionary<string, UIHealthReportEntry> entries, TimeSpan totalDuration)
        {
            Entries = entries;
            TotalDuration = totalDuration;
        }

        public static UIHealthReport CreateFrom(HealthReport report)
        {
            var uiReport = new UIHealthReport(new Dictionary<string, UIHealthReportEntry>(), report.TotalDuration)
            {
                Status = (UIHealthStatus) report.Status,
            };

            foreach (var item in report.Entries)
            {
                var entry = new UIHealthReportEntry
                {
                    Data = item.Value.Data,
                    Description = item.Value.Description,
                    Duration = item.Value.Duration,
                    Status = (UIHealthStatus) item.Value.Status
                };

                if (item.Value.Exception != null)
                {
                    var message = item.Value.Exception?.Message;

                    entry.Exception = message;
                    entry.Description = item.Value.Description ?? message;
                }

                uiReport.Entries.Add(item.Key, entry);
            }

            return uiReport;
        }
    }

    public enum UIHealthStatus
    {
        Unhealthy = 0,
        Degraded = 1,
        Healthy = 2
    }

    public class UIHealthReportEntry
    {
        public IReadOnlyDictionary<string, object>? Data { get; set; }
        public string? Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Exception { get; set; }
        public UIHealthStatus Status { get; set; }
    }
}