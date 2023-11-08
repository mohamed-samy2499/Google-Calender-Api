using CalenderSystem.Application.Feature.Events.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Commands.Validators
{
    public class AddEventValidator : AbstractValidator<AddEventCommand>
    {
        public AddEventValidator() { ApplyValidationsRules(); }
        public void ApplyValidationsRules()
        {

            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage("Summary can't be Empty")
                .NotNull().WithMessage("Summary is Required")
                .MinimumLength(3).WithMessage("Summary Minimum Length is 3 characters ")
                .MaximumLength(1000).WithMessage("Summary Maximum Length is 1000 characters ");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description can't be Empty")
                .NotNull().WithMessage("Description is Required")
                .MaximumLength(1000).WithMessage("Description Maximum Length is 1000 characters ");

            RuleFor(x => x.StartDateTime)
                .NotEmpty().WithMessage("StartDateTime can't be Empty")
                .NotNull().WithMessage("StartDateTime is Required");

            RuleFor(x => x.EndDateTime)
                .NotEmpty().WithMessage("EndDateTime can't be Empty")
                .NotNull().WithMessage("EndDateTime is Required")
                .GreaterThan(eventDto => eventDto.StartDateTime)
                .WithMessage("End time must be later than start time.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location can't be Empty")
                .NotNull().WithMessage("Location is Required");

            RuleFor(x => x.StartDateTime)
                .Must(BeValidStartDateTime)
                .WithMessage("Start time must not be in the past and should not be on a Saturday or Friday.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken can't be Empty")
                .NotNull().WithMessage("RefreshToken is Required");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId can't be Empty")
                .NotNull().WithMessage("UserId is Required");

        }
        private bool BeValidStartDateTime(DateTime startTime)
        {
            // Check if it's in the past
            if (startTime < DateTime.Now)
                return false;

            // Check if it's a Saturday or Friday
            if (startTime.DayOfWeek == DayOfWeek.Saturday || startTime.DayOfWeek == DayOfWeek.Friday)
                return false;

            return true;
        }
    }
}
