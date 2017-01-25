function translateCountry(country)
{
	var tempCountry = document.createElement("select");
	for (var i = 0; i < country.length; i++)
	{
		var opt = document.createElement("option");
		opt.text = country.options[i].text;
		opt.value = swapValues(country.options[i].value);
		opt.selected = country.options[i].selected;
		tempCountry.options.add(opt);
	}
	return tempCountry;
}

function swapValues(value)
{
	switch (value)
	{
		case "US":
			return "223";
			break;
		case "GB":
			return "222";
			break;
		case "IE":
			return "103";
			break;
		case "DE":
			return "81";
			break;
		case "CA":
			return "38";
			break;
		case "FR":
			return "73";
			break;
		case "AF":
			return "1";
			break;
		default:
			return "";
			break;
	}
}