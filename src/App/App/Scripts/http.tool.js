function httpRequestBuilder(method, url, data, cache)
{
	var auth = window.sessionStorage.getItem("auth-token");
	var tenant = window.sessionStorage.getItem("tenant");

	if (tenant == null)
	    tenant = "0";
	if (auth == null)
		auth = "";

	if (url.indexOf(REST_SERVER_HOSTNAME) == -1)
	    url = REST_SERVER_HOSTNAME + url;

	return request = {
		method: method,
		url: url,
        cache: ((cache) ? cache : false),
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
    if (response.status == 401) {
        window.location.href = "login.html"
        return;
    }

    if (REST_REDIRECT_ON_ERROR)
        window.location.replace("error.html?status=" + status);
    else {
        alert("HTTP " + response.status + " " + response.statusText + " // " + ((response.data != null) ? response.data.Message : ""));
    }
}

function getQueryString(param) {
    var url = location.href.slice(window.location.href.lastIndexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}