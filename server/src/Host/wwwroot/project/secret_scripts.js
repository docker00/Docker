$(function () {
    'use strict';

    window.secretListOperateEvents = {
        'click .edit': function (e, value, row, index) {
            window.location.href = "/Secret/Edit/?secretId=" + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить эту запись?")) {
                var data = { secretId: row.id };
                $.get('/Secret/Delete', data, function (res) {
                    if (res) {
                        $('#secretList_data_table').bootstrapTable('remove', {
                            field: 'id',
                            values: [row.id]
                        });
                    }
                });
            }
        }
    };
});

function secretListActionFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Детальная информация">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function secretListValueFormatter(value, row) {
    if (value.length > 30) {
        value = value.substr(0, 30) + "...";
    }
    return value;
}