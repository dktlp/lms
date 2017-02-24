function httpRequestBuilder(method, url, data)
{
	var auth = window.sessionStorage.getItem("auth-token");
	var tenant = window.sessionStorage.getItem("tenant");

	if (tenant == null)
	    tenant = "0";
	if (auth == null)
		auth = "";

	return request = {
		method: method,
		url: REST_SERVER_HOSTNAME + url,
		headers: {
			"Content-Type": "application/json",
			"Authorization": (auth != "") ? "Bearer " + auth : "",
			"LMS-Tenant-Identifier": tenant,
			"LMS-App-Name": APP_NAME,
			"LMS-App-Version": APP_VERSION
		},
		data: data,
        withCredentials: true
	}
}

function httpErrorHandler(response) {
    if (REST_REDIRECT_ON_ERROR)
        window.location.replace("error.html?status=" + status);
    else {
        alert("HTTP " + response.status + " " + response.statusText + " // " + ((response.data != null) ? response.data.Message : ""));
    }
}