﻿namespace Ray.Core.Logging
{
    public static class LogEventIds
    {
        private const int StartId = 100000;
        public const int GrainActivateId = StartId + 101;
        public const int GrainDeactivateId = StartId + 102;
        public const int GrainStateRecoveryId = StartId + 103;
        public const int GrainRaiseEvent = StartId + 104;
        public const int GrainSnapshot = StartId + 105;
        public const int MessagePublish = StartId + 106;

        public const int FollowGrainActivateId = StartId + 201;
        public const int FollowGrainDeactivateId = StartId + 202;
        public const int FollowGrainStateRecoveryId = StartId + 203;
        public const int FollowEventProcessing = StartId + 204;
        public const int FollowGrainSaveSnapshot = StartId + 205;
        public const int FollowGrainEventHandling = StartId + 206;
         
        public const int TransactionGrainTransactionFlow = StartId + 301;
        public const int TransactionGrainCurrentInput = StartId + 302;
        public const int TransactionGrainCurrentProcessing= StartId + 303;
    }
}
