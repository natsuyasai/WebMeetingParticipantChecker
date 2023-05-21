using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Monitoring
{
    internal interface IMonitoringFacade
    {
        IEnumerable<MonitoringInfo> GetMonitoringInfos();
        bool IsAllJoin();
        bool IsEnableAutoScroll();
        void Pause();
        void RegisterMonitoringTargets(IEnumerable<string> targetUsers);
        void Resume();
        Task<bool> SelectZoomParticipantElement(MonitoringType.Target targetType, Action onDetectedTargetElemetCallback);
        void SetEnableAutoScroll(bool val);
        void SetParticipantAuto(int target);
        Task StartMonitoring(Action onJoinStateChangeCallback);
        void StopMonitoring();
        void SwitchingParticipantState(int target);
    }
}