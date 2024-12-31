using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.DialogDetails;
using CoreBot.Helpers;
using CoreBot.Models;
using Data.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs;

public class PlaceAppointmentDialog : ComponentDialog
{
    private readonly AppointmentDataService _appointmentDataService;
    private readonly DoctorDataService _doctorDataService;

    private const string TimeMessage = "At what time would you like to make an appointment?";
    private const string NameMessage = "What name can I put that under?";
    private const string ReasonMessage = "For what reason do you need a consultation?";
    private const string DoctorMessage = "Which doctor would you like to make an appointment with?";
    private const string ContactMessage = "How can we contact you?";

    private const string OkMessage = "Your appointment has been made.";

    public PlaceAppointmentDialog(AppointmentDataService appointmentDataService, DoctorDataService doctorDataService) : base(nameof(PlaceAppointmentDialog))
    {
        _appointmentDataService = appointmentDataService;
        _doctorDataService = doctorDataService;

        AddDialog(new TextPrompt(nameof(TextPrompt)));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

        var waterfallSteps = new WaterfallStep[]
        {
            FirstTimeStepAsync,
            TimeNameStepAsync,
            NameReasonStepAsync,
            ReasonDoctorStepAsync,
            DoctorContactStepAsync,
            ContactActStepAsync,
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> FirstTimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (PlaceAppointmentDetails)stepContext.Options;

        if (details.Time == null)
        {
            var promptMessage = MessageFactory.Text(TimeMessage, TimeMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Time, cancellationToken);
    }

    private async Task<DialogTurnResult> TimeNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (PlaceAppointmentDetails)stepContext.Options;
        details.Time = (string)stepContext.Result;

        if (details.Name == null)
        {
            var promptMessage = MessageFactory.Text(NameMessage, NameMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Name, cancellationToken);
    }

    private async Task<DialogTurnResult> NameReasonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (PlaceAppointmentDetails)stepContext.Options;
        details.Time = (string)stepContext.Result;

        if (details.Reason == null)
        {
            var promptMessage = MessageFactory.Text(ReasonMessage, ReasonMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Reason, cancellationToken);
    }

    private async Task<DialogTurnResult> ReasonDoctorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (PlaceAppointmentDetails)stepContext.Options;
        details.Reason = (string)stepContext.Result;

        var doctors = await _doctorDataService.GetDoctorsAsync();

        var doctorList = doctors.Select(d => new AdaptiveSubmitAction
        {
            Title = d.Name,
            Data = d.Id.ToString()
        }).ToList();

        return await ChoicePromptHelper.PromptChoiceAsync(DoctorMessage, doctorList, stepContext, cancellationToken);
    }

    private async Task<DialogTurnResult> DoctorContactStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (PlaceAppointmentDetails)stepContext.Options;
        details.DocterId = int.Parse(((FoundChoice)stepContext.Result).Value);

        if (details.Contact == null)
        {
            var promptMessage = MessageFactory.Text(ContactMessage, ContactMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Contact, cancellationToken);
    }

    private async Task<DialogTurnResult> ContactActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (PlaceAppointmentDetails)stepContext.Options;
        details.Contact = (string)stepContext.Result;

        await _appointmentDataService.InsertAppointmentAsync(new Appointment
        {
            PatientName = details.Name,
            Contact = details.Contact,
            Reason = details.Reason,
            Time = details.Time,
            DoctorId = details.DocterId
        });
        await stepContext.Context.SendActivityAsync(MessageFactory.Text(OkMessage), cancellationToken);

        return await stepContext.EndDialogAsync(details, cancellationToken);
    }
}