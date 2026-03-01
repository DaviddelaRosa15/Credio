using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Dtos.Client
{
    public class ClientBasicDTO
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public int Age { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }
    }
}
