$(function () {
    'use strict';

    $(document).on('click', '.glyphicon glyphicon-trash', function (e) {
        var current_target = $(e.currentTarget);
        $.get(current_target.attr('href'), function (res) {
            if (res === true) {
                current_target.closest('tr').remove();
            }
        });
    });

    window.clientListOperateEvents = {
        'click .details': function (e, value, row, index) {
            window.location.href = "/Client/Details/?clientId=" + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить эту запись?")) {
                var data = { clientId: row.id };
                $.get('/Client/Delete', data, function (res) {
                    if (res) {
                        $('#clientList_data_table').bootstrapTable('remove', {
                            field: 'id',
                            values: [row.id]
                        });
                    }
                });
            }
        }
    };
});

function clientListActionFormatter(value, row) {
    return [
        '<a class="details" href="javascript:void(0)" title="Детальная информация">',
        '<i class="glyphicon glyphicon-eye-open"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function clientCheckboxFormatter(value) {
    var checkbox = '<input type="checkbox" disabled="disabled" ' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}