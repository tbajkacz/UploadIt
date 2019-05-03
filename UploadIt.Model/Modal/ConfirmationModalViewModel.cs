using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadIt.Model.Modal
{
    public class ConfirmationModalViewModel
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string CloseButton { get; set; }

        public string ConfirmationButton { get; set; }
    }
}
