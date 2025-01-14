using FinCtrlLibrary.Validators;

namespace FinCtrlLibrary.Models
{
    public class Category : Validator
    {
        private const int _nameMaxLength = 30;

        public int Id { get; set; }
        public string Name { get; set; }

        public Category(string name)
        {
            Id = 0;
            Name = name;
            Validate();
        }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
            Validate();
        }

        protected override void Validate()
        {
            IdValidation(Id, nameof(Id));
            NotEmptyStringValidation(nameof(Name), Name);
            NotEmptyStringLengthValidation(nameof(Name), Name, _nameMaxLength);
        }
    }
}
