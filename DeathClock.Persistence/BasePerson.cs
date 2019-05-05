using System;
using System.ComponentModel.DataAnnotations;

namespace DeathClock.Persistence
{
    /// <summary>
    /// Contains abstract person details.
    /// </summary>
    public abstract class BasePerson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        [Required]
        public bool IsDead { get; set; }

        [Required]
        [MaxLength(Constants.MAX_TITLE_LENGTH)]
        public string Title { get; set; }

        [Required]

        [MaxLength(Constants.MAX_DATASET_LENGTH)]
        public string DataSet { get; set; }

        [Required]
        public DateTime RecordedDate { get; set; }

        /// <summary>
        /// Gets or sets when this person should be updated.
        /// </summary>
        [Required]
        public DateTime UpdateDate { get; set; }

        public int Age
        {
            get
            {
                return (int)Math.Floor((DateTime.Now - BirthDate).TotalDays / 365);
            }
        }
    }
}
