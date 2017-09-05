

$(function () {
    $('.panel').on('click', 'form button:has(.glyphicon-ok)', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var form = $(e.currentTarget).closest('form');
        var attr = form.attr('data-ajax-success');
        if (attr !== undefined && attr !== "") {
            form.submit();
            return;
        }

        var data = form.serialize();
        $.post(form.attr('action'), data, function (res) {
            if (res) {
                var btn = createAddBtn(e);
                $(e.currentTarget).closest('.panel-footer').html(btn);
                $('#object_endpoint_data_table').bootstrapTable('refresh');
            }
        });
    }).on('click', 'form button:has(.glyphicon-ban-circle)', function (e) {
        var btn = createAddBtn(e);
        $(e.currentTarget).closest('.panel-footer').html(btn);
    });
});

function back() {
    $('#object_endpoint_data_table').bootstrapTable();
    $('#ObjectEndpoints_collapse').attr('aria-expanded', 'true');
    $('#ObjectEndpoints_collapse').attr('class', '');
}

window.objectEndpointEvents = {
    'click .permission': function (e, value, row, index) {
        var data = { objectEndpointId: row.id, objectId: row.objectId };
        $.get('/Object/_ObjectEndpointPermissionListPartial', data, function (res) {
            if (res) {
                $('.panel-footer').hide();
                $('#ObjectEndpoints .panel-body').html(res);
            }
        });
    },
    'click .subject': function (e, value, row, index) {
        var data = { objectId: row.objectId, objectEndpointId: row.id };
        $.get('/Object/_SubjectListPartial', data, function (res) {
            if (res) {
                $('.panel-footer').hide();
                $('#ObjectEndpoints .panel-body').html(res);
            }
        });
    },
    'click .edit': function (e, value, row, index) {
        var data = { objectEndpointId: row.id };
        $.get('/Object/_ObjectEndpointEditPartial', data, function (res) {
            if (res) {
                $('.panel-footer').hide();
                $('#ObjectEndpoints .panel-body').html(res);
            }
        });
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить?")) {
            var data = { Id: row.id };

            $.post('/ObjectEndpoint/Delete', data, function (res) {
                if (res) {
                    $('#object_endpoint_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function objectEndpointFormatter(value, row) {
    return [
        '<a class="permission btn btn-default" href="javascript:void(0)" title="Разрешения">',
        '<i class="glyphicon glyphicon-user"></i> Разрешения',
        '</a>  ',
        '<a class="subject btn btn-default" href="javascript:void(0)" title="Субъекты">',
        '<i class="glyphicon glyphicon-user"></i> Субъекты',
        '</a>  ',
        '<a class="edit" href="javascript:void(0)" title="Редактировать"> ',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function createAddBtn(e) {
    var form = $(e.currentTarget).closest('form');
    var addBtnAction = form.attr('action');
    var hidden_inputs = $('input[type="hidden"]', form);
    var action_parts = addBtnAction.split('/');
    if (hidden_inputs.length > 0) {
        if (action_parts.length > 3) {
            if (action_parts[4] === "") {
                addBtnAction += "?";
            }
            else {
                addBtnAction += "&";
            }
        }
        else {
            addBtnAction += "/?";
        }
        $('input[type="hidden"]', form).each(function (index, element) {
            var elem = $(element);
            var elem_name = elem.attr('name');
            if (elem_name !== undefined) {
                elem_name = elem_name.substr(0, 1).toLowerCase() + elem_name.substring(1, elem_name.length);
                addBtnAction += elem_name + "=" + elem.val() + "&";
            }
        });
        if (addBtnAction.substr(addBtnAction.length - 1) === "&" || addBtnAction.substr(addBtnAction.length - 1) === "?") {
            addBtnAction = addBtnAction.substring(0, addBtnAction.length - 1);
        }
    }
    var addBtn = [
        '<a href="' + addBtnAction + '" class="btn btn-default" id="' + action_parts[2] + '"',
        'data-ajax="true" data-ajax-method="GET" data-ajax-mode="replace-with" data-ajax-update="#' + action_parts[2] + '">',
        'Добавить&nbsp;<span class="glyphicon glyphicon-plus"></span>',
        '</a>'
    ].join('');
    return addBtn;
}