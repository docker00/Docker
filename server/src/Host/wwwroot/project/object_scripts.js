window.objectListOperateEvents = {
    'click .details': function (e, value, row, index) {
        window.location.href = '/Object/Details?objectId=' + row.id;
    },
    'click .edit': function (e, value, row, index) {
        window.location.href = '/Object/Edit?objectId=' + row.id;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить объект?")) {
            var data = { objectId: row.id };

            $.get('/Object/Delete', data, function (res) {
                if (res) {
                    $('#object_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }
    },
};

function objectListActionformatter(value, row) {
    return [
        '<a class="details" href="javascript:void(0)" title="Детальная информация"">',
        '<i class="glyphicon glyphicon-eye-open"></i>',
        '</a>  ',
        '<a class="edit" href="javascript:void(0)" title="Редактировать">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function GetChildrenPermissionEndpoint(e, objectEndpointId) {
    var children = $(e).closest('li');

    $(children).find('ul').remove();

    if ($(children).find(' > span').attr('class') === 'glyphicon glyphicon-minus') {
        $(children).find(' > span').attr('class', 'glyphicon glyphicon-plus');
    } else {
        var data;
        data = { tableId: e.id, type: $(children).attr('name'), objectEndpointId: objectEndpointId };
        $.get('/Object/GetChildrenPermissionEndpoint', data, function (res) {
            if (res) {
                $(children).append(res);
                $(children).find(' > span').attr('class', 'glyphicon glyphicon-minus');
            }
        });
    }
}


function GetChildrenPermissionObject(e, objectId) {
    var children = $(e).closest('li');

    $(children).find('ul').remove();

    if ($(children).find('span').attr('class') === 'glyphicon glyphicon-minus') {
        $(children).find('span').attr('class', 'glyphicon glyphicon-plus');
    } else {
        var data;
        data = { tableId: e.id, type: $(children).attr('name'), objectId: objectId };
        $.get('/Object/GetChildrenPermissionObject', data, function (res) {
            if (res) {
                $(children).find('span').attr('class', 'glyphicon glyphicon-minus');
                $(children).append(res);
            }
        });
    }
}

function ChangeStatus(objectId, obj) {
    var data = { objectId: objectId, status: obj.checked };

    $.get('/Object/ChangeStatus', data, function (res) {
        if (!res) {
            alert('Отсутствует Клиент у Объекта, зайти в объект и создать клиента');
            obj.checked = !obj.checked;
        }
    });
}

function enabledCheckboxFormatter(value, row) {
    var checkbox = '<input type="checkbox" ' + (value === true ? "checked" : "") + ' value="true" onchange="ChangeStatus(\'' + row.id + '\', this)"/>';
    return checkbox;
}