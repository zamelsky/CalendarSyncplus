﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;
using OutlookGoogleSyncRefresh.Domain.Models;

namespace OutlookGoogleSyncRefresh.Application.Utilities
{
    public static class AppointmentHelper
    {
        public static void LoadSourceId(this Appointment calendarAppointment, string sourceCalendarId)
        {
            var key = GetSourceEntryKey(sourceCalendarId);
            object value;
            if (calendarAppointment.ExtendedProperties.TryGetValue(key, out value))
            {
                calendarAppointment.SourceId = value.ToString();
            }
        }

        public static string GetSourceEntryKey(this Appointment calendarAppointment)
        {
            return GetSourceEntryKey(calendarAppointment.CalendarId);
        }

        public static string GetSourceEntryKey(string sourceCalendarId)
        {
            return GetMD5Hash(sourceCalendarId); 
        }


        public static string GetSourceId(this Appointment calenderAppointment)
        {
            if (calenderAppointment.IsRecurring)
            {
                if (calenderAppointment.StartTime != null)
                {
                    return string.Format("{0}_{1}", calenderAppointment.AppointmentId,
                        calenderAppointment.StartTime.Value.ToString("yy-mm-dd"));
                }
            }
            return calenderAppointment.AppointmentId;
        }

        public static bool CompareSourceId(this Appointment calendarAppointment, Appointment otherAppointment)
        {
            if (calendarAppointment.IsRecurring)
            {
                return calendarAppointment.GetSourceId().Equals(otherAppointment.SourceId);
            }
            return calendarAppointment.AppointmentId.Equals(otherAppointment.SourceId);
        }

        public static string GetDescriptionData(this Appointment calenderAppointment, bool addAttendees)
        {
            var additionDescription = new StringBuilder(string.Empty);
            additionDescription.Append(calenderAppointment.Description);
            if (!addAttendees)
            {
                return additionDescription.ToString();
            }
            //Start Header
            additionDescription.AppendLine("==============================================");
            additionDescription.AppendLine(string.Empty);
            //Add Organiser
            additionDescription.AppendLine("Organizer");
            additionDescription.AppendLine(calenderAppointment.Organizer.Name);
            additionDescription.AppendLine(string.Empty);

            //Add Required Attendees
            additionDescription.AppendLine("Required Attendees:");
            if (calenderAppointment.RequiredAttendees != null)
            {
                foreach (var requiredAttendee in calenderAppointment.RequiredAttendees)
                {
                    additionDescription.AppendLine(requiredAttendee.Name);
                }
            }

            additionDescription.AppendLine(string.Empty);
            //Add Optional Attendees
            additionDescription.AppendLine("Optional Attendees:");
            if (calenderAppointment.OptionalAttendees != null)
            {
                foreach (var requiredAttendee in calenderAppointment.OptionalAttendees)
                {
                    additionDescription.AppendLine(requiredAttendee.Name);
                }
            }

            additionDescription.AppendLine(string.Empty);
            //Close Header
            additionDescription.AppendLine("==============================================");

            return additionDescription.ToString();
        }

        private static string SplitAttendees(string attendees)
        {
            var attendeesBuilder = new StringBuilder(string.Empty);
            if (string.IsNullOrEmpty(attendees))
            {
                return attendees;
            }

            foreach (string attendeeName in attendees.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                attendeesBuilder.AppendLine(attendeeName.Trim());
            }
            return attendees;
        }

        public static OlBusyStatus GetOutlookBusyStatus(this Appointment calendarAppointment)
        {
            if (calendarAppointment.BusyStatus == BusyStatusEnum.Busy)
                return OlBusyStatus.olBusy;
            if (calendarAppointment.BusyStatus == BusyStatusEnum.Free)
                return OlBusyStatus.olFree;
            if (calendarAppointment.BusyStatus == BusyStatusEnum.OutOfOffice)
                return OlBusyStatus.olOutOfOffice;
            if (calendarAppointment.BusyStatus == BusyStatusEnum.Tentative)
                return OlBusyStatus.olTentative;
            return OlBusyStatus.olFree;
        }

        public static void SetBusyStatus(this Appointment calendarAppointment, OlBusyStatus busyStatus)
        {
            if (busyStatus == OlBusyStatus.olBusy)
                calendarAppointment.BusyStatus = BusyStatusEnum.Busy;
            else if (busyStatus == OlBusyStatus.olFree)
                calendarAppointment.BusyStatus = BusyStatusEnum.Free;
            else if (busyStatus == OlBusyStatus.olOutOfOffice)
                calendarAppointment.BusyStatus = BusyStatusEnum.OutOfOffice;
            else if (busyStatus == OlBusyStatus.olTentative)
                calendarAppointment.BusyStatus = BusyStatusEnum.Tentative;

        }

        private static string GetMD5Hash(string stringToHash)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] retVal = md5.ComputeHash(Encoding.Unicode.GetBytes(stringToHash));

                foreach (byte byteval in retVal)
                {
                    sb.Append(byteval.ToString("x2"));
                }
            }
            return sb.ToString();
        }
    }
}
