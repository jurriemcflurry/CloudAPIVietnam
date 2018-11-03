﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class Formulieren
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        public string Name { get; set; }
        public string Region { get; set; }
        public FormTemplateModel FormTemplate { get; set; }

        [ForeignKey("FormulierenId")]
        public List<FormContent> FormContent { get; set; }
    }


}