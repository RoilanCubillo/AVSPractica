using System;

namespace UltraERP.BusinessEntities
{
    public class EN_ScheduleSegment
    {
        #region Fields
        private int iD;
        private int hQID;
        private int scheduleID;
        private int startDay;
        private int endDay;
        private DateTime startTime;
        private DateTime endTime;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the EN_ScheduleSegment class.
        /// </summary>
        public EN_ScheduleSegment()
        {
        }
        /// <summary>
        /// Initializes a new instance of the EN_ScheduleSegment class.
        /// </summary>
        public EN_ScheduleSegment(int hQID, int scheduleID, int startDay, int endDay, DateTime startTime, DateTime endTime)
        {
            this.hQID = hQID;
            this.scheduleID = scheduleID;
            this.startDay = startDay;
            this.endDay = endDay;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        /// <summary>
        /// Initializes a new instance of the EN_ScheduleSegment class.
        /// </summary>
        public EN_ScheduleSegment(int iD, int hQID, int scheduleID, int startDay, int endDay, DateTime startTime, DateTime endTime)
        {
            this.iD = iD;
            this.hQID = hQID;
            this.scheduleID = scheduleID;
            this.startDay = startDay;
            this.endDay = endDay;
            this.startTime = startTime;
            this.endTime = endTime;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        /// <summary>
        /// Gets or sets the HQID value.
        /// </summary>
        public virtual int HQID
        {
            get { return hQID; }
            set { hQID = value; }
        }

        /// <summary>
        /// Gets or sets the ScheduleID value.
        /// </summary>
        public virtual int ScheduleID
        {
            get { return scheduleID; }
            set { scheduleID = value; }
        }

        /// <summary>
        /// Gets or sets the StartDay value.
        /// </summary>
        public virtual int StartDay
        {
            get { return startDay; }
            set { startDay = value; }
        }

        /// <summary>
        /// Gets or sets the EndDay value.
        /// </summary>
        public virtual int EndDay
        {
            get { return endDay; }
            set { endDay = value; }
        }

        /// <summary>
        /// Gets or sets the StartTime value.
        /// </summary>
        public virtual DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Gets or sets the EndTime value.
        /// </summary>
        public virtual DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public virtual int Hour { get { return startTime != null ? startTime.Hour : 0; } }

        public virtual int Minute { get { return startTime != null ? startTime.Minute : 0; } }
        #endregion
    }
}
