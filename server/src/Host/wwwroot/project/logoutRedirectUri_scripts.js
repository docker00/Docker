$(function () {
    'use strict';

    window.logoutRedirectUriListOperateEvents = {
        'click .edit': function (e, value, row, index) {
            window.location.href = "/PostLogoutRedirectUri/Edit/?logoutRedirectUriId=" + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить эту запись?")) {
                var data = { logoutRedirectUriId: row.id };
                $.get('/PostLogoutRedirectUri/Delete', data, function (res) {
                    if (res) {
                        $('#postLogoutRedirectUriList_data_table').bootstrapTable('remove', {
                            field: 'id',
                            values: [row.id]
                        });
                    }
                });
            }
        }
    };
});

function logoutRedirectUriListActionFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактирование">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}
