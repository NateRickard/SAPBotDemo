﻿#load "Utils.csx"

using System;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;

[Serializable]
public class MonthSelectionDialog : IDialog<int>
{
	DateTime minDate;
	DateTime maxDate;

	public MonthSelectionDialog (DateTime minDate, DateTime maxDate)
	{
		this.minDate = minDate;
		this.maxDate = maxDate;
	}

	public async Task StartAsync (IDialogContext context)
	{
		await showMonthPrompt (context, minDate, maxDate);
	}

	private async Task showMonthPrompt (IDialogContext context, DateTime dateRangeMin, DateTime dateRangeMax)
	{
		await context.PostAsync ($"It looks like you asked for that sales between {dateRangeMin.ToShortDateString ()} and {dateRangeMax.ToShortDateString ()}");
		await context.PostAsync ($"I can only return sales data for a single month currently.");

		var monthOptions = new List<string> ();

		DateTime dtNext = dateRangeMin;

		while (dtNext < dateRangeMax)
		{
			monthOptions.Add (Utils.GetMonthName (dtNext.Month));
			dtNext = dtNext.AddMonths (1);
		}

		PromptDialog.Choice<string> (
			context,
			AfterMenuSelection,
			monthOptions,
			"Which month would you like to view sales data for?",
			"Ooops, what you wrote is not a valid month, please try again",
			3,
			PromptStyle.Auto,
			monthOptions);
	}

	private async Task AfterMenuSelection (IDialogContext context, IAwaitable<string> result)
	{
		try
		{
			var monthName = await result;
			var selectedMonth = Utils.GetMonthNumber (monthName);

			await context.PostAsync ($"Great, I'll show you results for {monthName}");

			context.Done (selectedMonth);
		}
		catch (TooManyAttemptsException ex)
		{
			context.Fail (ex);
		}
	}
}