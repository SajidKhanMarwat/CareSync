using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareSync.DataLayer.Entities.Lab_Entities
{
    public class T_UserLabAssistant:BaseEntity
    {
        [Key]
        public int UserLabAssistantID { get; set; }
        public required string LabAssistantId { get; set; }
        public required int LabId { get; set; }
    }
}
