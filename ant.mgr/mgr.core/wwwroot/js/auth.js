function auth() {
    QQT.ajax('/Admin/Role/GetSystemUserActions', 'Get', null, false, false)
    .done(function (response) {
        $('.loading').show();
        var result = response.Data;
        if (result.IsGod) {
            $('.loading').hide();
            return;
        }

        if (result.ActionList && result.ActionList.indexOf('index') < 0) {
            $('.loading').hide();
            window.location.href =
                window.appUrl +
                '/' +
                'Admin/Error' +
                '/' +
                'Http403';
            return;
            //mvcJump('Error', 'Http403', { acionInfo: window.location.pathname });
        }

        $('.authorization').each(function (index, item) {
            var actionId = $(item).attr('action-id') || '';
            var isPermit = false;
            if (result.ActionList && result.ActionList.indexOf(actionId) >= 0) {
                isPermit = true;
            }
            if (!isPermit) {
                $(item).remove();
            }
        });
        setTimeout(function () {
            $('.loading').hide();
        },200);
    });
}

auth();