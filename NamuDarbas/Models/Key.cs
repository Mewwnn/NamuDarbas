using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NamuDarbas.Models
{
    public class KeyInfo
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            public string Algorithm { get; set; }
            
            [Required]
            public string Message { get; set; }

            [Required]
            public string PublicKey { get; set; }

            [Required]
            public string PrivateKey { get; set; }
        }
    
}