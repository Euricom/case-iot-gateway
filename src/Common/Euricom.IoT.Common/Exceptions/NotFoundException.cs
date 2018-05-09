using System;

namespace Euricom.IoT.Common.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException()
        {
            
        }

        public NotFoundException(string message): base(message)
        {
            
        }
    }
}
