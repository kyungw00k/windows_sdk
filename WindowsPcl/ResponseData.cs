﻿using AdjustSdk.Pcl;
using System.Collections.Generic;

namespace AdjustSdk
{
    public class ResponseData
    {
        #region Set by SDK

        // the kind of activity (ActivityKind.Session etc.)
        public ActivityKind ActivityKind { get; set; }

        // true when the activity was tracked successfully
        // might be true even if response could not be parsed
        public bool Success { get; set; }

        // true if the server was not reachable and the request will be tried again later
        public bool WillRetry { get; set; }

        #endregion Set by SDK

        #region Set by server or SDK

        // nil if activity was tracked successfully and response could be parsed
        // might be not nil even when activity was tracked successfully
        public string Error;

        #endregion Set by server or SDK

        #region Set by server

        // the following attributes are only set when error is nil
        // (when activity was tracked successfully and response could be parsed)

        // tracker token of current device
        public string TrackerToken;

        // tracker name of current device
        public string TrackerName;

        // tracker network
        public string Network;

        // tracker campaign
        public string Campaign;

        // tracker adgroup
        public string AdGroup;

        // tracker creative
        public string Creative;

        #endregion Set by server

        // returns human readable version of activityKind
        // (session, event, revenue), see above
        public string ActivityKindString { get { return ActivityKindUtil.ToString(ActivityKind); } }

        #region internal

        public void SetResponseData(Dictionary<string, string> jsonDict, string jsonString)
        {
            if (jsonDict == null)
            {
                Error = Util.f("Failed to parse json response: {0}", jsonString);
                return;
            }

            jsonDict.TryGetValue("error", out Error);
            jsonDict.TryGetValue("tracker_token", out TrackerToken);
            jsonDict.TryGetValue("tracker_name", out TrackerName);
            jsonDict.TryGetValue("network", out Network);
            jsonDict.TryGetValue("campaign", out Campaign);
            jsonDict.TryGetValue("adgroup", out AdGroup);
            jsonDict.TryGetValue("creative", out Creative);
        }

        public void SetResponseError(string errorString)
        {
            Error = errorString;
            Success = false;
        }

        #endregion internal

        public override string ToString()
        {
            return Util.f("[kind: {0} success:{1} willRetry:{2} error:{3} trackerToken:{4} trackerName:{5} network:{6} campaign:{7} adgroup:{8} creative:{9}]",
                ActivityKindString,
                Success,
                WillRetry,
                Error.Quote(),
                TrackerToken,
                TrackerName.Quote(),
                Network.Quote(),
                Campaign.Quote(),
                AdGroup.Quote(),
                Creative.Quote());
        }

        public Dictionary<string, string> ToDic()
        {
            var responseDataDic = new Dictionary<string, string>(6)
            {
                { "activityKind", ActivityKindString },
                { "success", (Success? "true" : "false")},
                { "willRetry", (WillRetry? "true" : "false")}
            };

            addToDic(responseDataDic, "error", Error);
            addToDic(responseDataDic, "trackerToken", TrackerToken);
            addToDic(responseDataDic, "trackerName", TrackerName);
            addToDic(responseDataDic, "network", Network);
            addToDic(responseDataDic, "campaign", Campaign);
            addToDic(responseDataDic, "adgroup", AdGroup);
            addToDic(responseDataDic, "creative", Creative);

            return responseDataDic;
        }

        private void addToDic(Dictionary<string, string> dic, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dic.Add(key, value);
            }
        }
    }
}