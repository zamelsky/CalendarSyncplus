﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarSyncPlus.Services.Contacts.Interfaces;

namespace CalendarSyncPlus.Services.Contacts
{
    [Export(typeof(IContactsUpdateService))]
    public class ContactsUpdateService : IContactsUpdateService
    {
    }
}
