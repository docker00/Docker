$(function () {
    'use strict';

    window.scopeListOperateEvents = {
        'click .edit': function (e, value, row, index) {
            window.location.href = "/Scope/Edit/?scopeId=" + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить эту запись?")) {
                var data = { scopeId: row.id };
                $.get('/Scope/Delete', data, function (res) {
                    if (res) {
                        $('#scopeList_data_table').bootstrapTable('remove', {
                            field: 'id',
                            values: [row.id]
                        });
                    }
                });
            }
        }
    };
});

function scopeListActionFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактирование">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function scopeListCheckboxFormatter(value) {
    var checkbox = '<input type="checkbox" disabled="disabled" ' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}
