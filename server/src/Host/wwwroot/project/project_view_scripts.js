$(function () {
    'use strict';
    $(document).on('show.bs.collapse', '.collapse', function (e) {
        if ($(this).is(e.target)) {
            var current_target = $(e.currentTarget);
            var collapse_element = $('a[href="#' + current_target.attr('id') + '"]');
            collapse_element.find('span.glyphicon-plus').first().removeClass('glyphicon-plus').addClass('glyphicon-minus');
        }
    }).on('hide.bs.collapse', '.collapse', function (e) {
        if ($(this).is(e.target)) {
            var current_target = $(e.currentTarget);
            var collapse_element = $('a[href="#' + current_target.attr('id') + '"]');
            collapse_element.find('span.glyphicon-minus').first().removeClass('glyphicon-minus').addClass('glyphicon-plus');
        }
    });
    $('.panel').on('click', '.panel-footer button:has(.glyphicon-ok)', function (e) {
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
            if (res !== true) {
                form.replaceWith(res);
            }
            else {
                var form_action = form.attr('action');
                var action_parts = form_action.split('/');
                action_parts[2] = action_parts[2].replace("_Add", "Delete").replace("Partial", "");
                form_action = "/" + action_parts[1] + "/" + action_parts[2];
                if (action_parts.length > 3) {
                    form_action += "/" + action_parts[4];
                    if (action_parts[4] === "") {
                        form_action += "?";
                    }
                    else {
                        form_action += "&";
                    }
                }
                else {
                    form_action += "/?";
                }
                var table = form.closest('.panel').find('table');
                var tr = "<tr>";
                var td_count = 0;
                $('input,select', form).each(function (index, element) {
                    var elem = element;
                    if (elem.name !== undefined) {
                        var elem_name = elem.name;
                        if (elem_name.indexOf('Id') !== -1 || $(element).hasClass('action-param')) {
                            form_action += elem_name.substr(0, 1).toLowerCase() + elem_name.substring(1, elem_name.length)
                                + "=" + elem.value + "&";
                        }
                        if (elem.type !== undefined && elem.type !== "hidden" && table.find('th').length - 1 > td_count) {
                            var elem_value = elem.value;
                            if (elem.nodeName === "SELECT") {
                                elem_value = $(":selected", element).text();
                            }
                            tr += '<td align="center">' + elem_value + '</td>';
                            td_count++;
                        }
                    }
                });
                if (form_action.substr(form_action.length - 1) === "&" || form_action.substr(form_action.length - 1) === "?") {
                    form_action = form_action.substring(0, form_action.length - 1);
                }
                var deleteBtn = [
                    '<a href="' + form_action + '"',
                    ' class="glyphicon glyphicon-remove" title= "Удалить у клиента"></a > '
                ].join('');
                tr += '<td align="center">' + deleteBtn + '</td>';
                tr += "</tr>";

                if (table.find('tbody tr').length === 1 && table.find('tbody tr').first().children().length === 1) {
                    table.find('tbody tr').first().remove();
                }
                table.append(tr);
                var add_btn = createAddBtn(e);
                form.closest('.panel-footer').html(add_btn);
            }
        });
    }).on('click', 'form button:has(.glyphicon-ban-circle)', function (e) {
        var btn = createAddBtn(e);
        $(e.currentTarget).closest('.panel-footer').html(btn);
    }).on('click', 'table .glyphicon-remove', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var current_target = $(e.currentTarget);
        $.get(current_target.attr('href'), function (res) {
            var table = current_target.closest('table');
            if (res === true) {
                current_target.closest('tr').remove();
            }
            if (table.find('tbody tr').length === 0) {
                table.append('<tr><td align="center" colspan="2">Нет записей</tr>');
            }
        });
        });

    window.usersListOperateEvents = {
        'click .details': function (e, value, row, index) {
            window.location.href = '/Group/RemoveUser/?userId=' + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить пользователя " + row.email + " из группы?")) {
                var data = { userId: row.id };
                $.get('/User/Delete', data, function (res) {
                    if (res) {
                        $(e.currentTarget.closest('table')).bootstrapTable('remove', {
                            field: 'id',
                            values: [row.id]
                        });
                    }
                });
            }
        }
    };
});

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

function usersListActionFormatter(value, row) {
    return [
        '<a class="details" href="javascript:void(0)" title="Детальная информация">',
        '<i class="glyphicon glyphicon-eye-open"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function userListActivatedFormatter(value) {
    var checkbox = '<input type="checkbox" name="activated_checbox"' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}