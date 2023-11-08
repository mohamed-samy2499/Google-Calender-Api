using CalenderSystem.Application.Feature.Events.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Commands.Validators
{
    public class DeleteEventValidator : AbstractValidator<DeleteEventCommand>
    {
        public DeleteEventValidator() { ApplyValidationsRules(); }
        public void ApplyValidationsRules()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId can't be Empty")
                .NotNull().WithMessage("EventId is Required");
        }
    }
}
