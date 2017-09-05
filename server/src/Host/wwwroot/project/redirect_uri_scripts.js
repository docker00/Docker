$(function () {
    'use strict';

    window.redirectUriListOperateEvents = {
        'click .edit': function (e, value, row, index) {
            window.location.href = "/RedirectUri/Edit/?redirectUriId=" + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить эту запись?")) {
                var data = { redirectUriId: row.id };
                $.get('/RedirectUri/Delete', data, function (res) {
                    if (res) {
                        $('#redirectUriList_data_table').bootstrapTable('remove', {
                            field: 'id',
                            values: [row.id]
                        });
                    }
                });
            }
        }
    };
});

function redirectUriListActionFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактирование">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}
