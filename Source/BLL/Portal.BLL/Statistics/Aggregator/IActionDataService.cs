﻿// Copyright (c) Clickberry, Inc. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3. See License.txt in the project root for license information.

using Portal.Domain.StatisticContext;

namespace Portal.BLL.Statistics.Aggregator
{
    public interface IActionDataService
    {
        DomainActionData GetActionData(string userAgent, int statusCode);
    }
}