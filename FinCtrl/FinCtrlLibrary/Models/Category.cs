﻿using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinCtrlLibrary.Models
{
    public class Category : ValidatorClass
    {
        private const int _nameMaxLength = 30;

        [BsonId, JsonIgnore]
        public ObjectId _id { get; set; }
        [BsonIgnore, BsonElement("id")]
        public int Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }

        public Category()
        {

        }

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

        public override string ToString()
        {
            return $"{_id} - {Name}";
        }
    }
}
