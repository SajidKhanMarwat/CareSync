using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareSync.DataLayer.Entities.Doctor_Entities
{
    public class T_UserDocterAssistant:BaseEntity
    {
        [Key]
        public int UserDocterAssistantId { get; set; }
        public required string DoctorAssistantId { get; set; }
        public required int DoctorId { get; set; }
    }
}
