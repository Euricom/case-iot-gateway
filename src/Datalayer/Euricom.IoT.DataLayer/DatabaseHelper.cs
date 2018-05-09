using System;
using System.Collections.Generic;
using System.Text;
using Euricom.IoT.Common.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Euricom.IoT.DataLayer
{
    public static class DatabaseHelper
    {
        public static void HandleAlreadyExistsException(this Exception exception)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                // A custom exception of yours for concurrency issues
                throw new AlreadyExistsException();
            }

            if (exception is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null
                    && dbUpdateEx.InnerException.InnerException != null)
                {
                    if (dbUpdateEx.InnerException.InnerException is SqliteException sqlException)
                    {
                        switch (sqlException.SqliteErrorCode)
                        {
                            case 19: // constraint error
                                throw new AlreadyExistsException();
                        }
                    }
                }
            }
        }
    }
}
