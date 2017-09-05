
var userSelected = [];
$(function () {
    'use strict';

    //UsersList функции
    {
        window.usersListOperateEvents = {
            'click .details': function (e, value, row, index) {
                window.location.href = '/User/Details/?userId=' + row.id;
            },
            'click .remove': function (e, value, row, index) {
                if (confirm("Вы действительно хотите удалить учетную запись " + row.email + "?")) {
                    var data = { userId: row.id };
                    $.get('/User/Delete', data, function (res) {
                        if (res) {
                            $('#usersList_data_table').bootstrapTable('remove', {
                                field: 'id',
                                values: [row.id]
                            });
                        }
                    });
                }
            },
            'click input[name="activated_checbox"]': function (e, value, row, index) {
                var data = { userId: row.id, activated: e.currentTarget.checked };
                if (!e.currentTarget.checked) {
                    if (!confirm("Вы точно хотите деактивировать учетную запись " + row.email + "?")) {
                        e.currentTarget.checked = true;
                        return;
                    }
                }
                $.get('/User/ActivatedEdit', data);
            }
        };

        window.usersSelectOperateEvents = {
            'click .remove': function (e, value, row, index) {
                $('#userMerge_data_table').bootstrapTable('remove', {
                    field: 'id',
                    values: [row.id]
                });
                userSelected.splice(userSelected.indexOf(row.id), 1);
                $("#usersList_data_table").bootstrapTable('refresh');
            }
        };

        window.apiKeyListOperateEvents = {
            'click .update': function (e, value, row, index) {
                if (confirm("Вы действительно хотите обновить ключ API у клиента " + row.clientId + "?")) {
                    var apiKeyForm = $("#apiKeyAddForm");
                    var action = "/" + apiKeyForm.attr('action').split('/')[1] + "/ApiKeyUpdate";
                    var data = { apiKeyId: row.id };
                    $.get(action, data, function (res) {
                        if (res === true) {
                            $('#apiKeyDataTable').bootstrapTable('refresh');
                            $("#clientListModal table").bootstrapTable('refresh');
                        }
                    });
                }
            },
            'click .remove': function (e, value, row, index) {
                if (confirm("Вы действительно хотите удалить ключ API у клиента " + row.clientId + "?")) {
                    var apiKeyForm = $("#apiKeyAddForm");
                    var action = "/" + apiKeyForm.attr('action').split('/')[1] + "/DeleteApiKey";
                    var data = { apiKeyId: row.id };
                    $.get(action, data, function (res) {
                        if (res === true) {
                            $('#apiKeyDataTable').bootstrapTable('remove', {
                                field: 'id',
                                values: [row.id]
                            });
                            $("#clientListModal table").bootstrapTable('refresh');
                        }
                    });
                }
            },
        };

        $('#usersList_data_table').on('expand-row.bs.table', function (e, index, row, detail) {
            var rolesContent = '<div class="col-md-3"><label class="row-md-12">Роли:</label>';
            if (row.roles.length > 0) {
                rolesContent += "<ul>";
                $.each(row.roles, function (index, role) {
                    rolesContent += "<li>" + role + "</li>";
                });
                rolesContent += "</ul>";
            }
            else {
                rolesContent += '<div class="row-md-12">Роли не назначены</div>';
            }
            rolesContent += '</div>';
            rolesContent += '<div class="col-md-3"><label class="row-md-12">Группы:</label>';
            if (row.roles.length > 0) {
                rolesContent += "<ul>";
                $.each(row.groups, function (index, group) {
                    rolesContent += "<li>" + group.name + "</li>";
                });
                rolesContent += "</ul>";
            }
            else {
                rolesContent += '<div class="row-md-12">Пользователь не состоит в группах</div>';
            }
            rolesContent += '</div>';
            $(detail).html(rolesContent);
        })
            .on('check.bs.table uncheck.bs.table', function (e, rows) {
                if (e.type === "check") {
                    userSelected.push(rows.id);
                    $("#userMerge_data_table").bootstrapTable('append', [rows]);
                }
                else {
                    userSelected.splice(userSelected.indexOf(rows.id), 1);
                    $("#userMerge_data_table").bootstrapTable('remove', { field: 'id', values: [rows.id] });
                }
            });

        $("#userMergeModal").on('click', '.modal-footer button:has(.glyphicon-ok)', function () {
            if (userSelected.length > 1) {
                var ids = "";
                $.each(userSelected, function (index, element) {
                    ids += "usersIds=" + element + "&";
                });
                ids = ids.slice(0, ids.length - 1);
                window.location.href = '/User/Merge/?' + ids;
            }
        });

        $("#userMergeForm").on('click', 'table .glyphicon-remove', function (e) {
            $(e.currentTarget).closest('tr').remove();
        });
    }

    //UserDetails функции
    {
        $("#profile").on("click", "#userPasswordEditLink", function (e) {
            e.preventDefault();
            e.stopPropagation();
            var action = $(e.currentTarget).attr('href');
            $.get(action, function (res) {
                $(e.currentTarget).parent().html(res);
            });
        });

        $("#profile").on('click', '#userPasswordContent button:has(.glyphicon-ok)', function (e) {
            var current_target = $(e.currentTarget);
            var data = current_target.closest('form').serialize();
            var action = current_target.closest('form').attr('action');
            $.post(action, data, function (res) {
                if (res === true) {
                    current_target.closest('form').replaceWith('<a id="userPasswordEditLink" href="' + action + '">Изменить пароль</a>');
                }
            });
        }).on('click', '#userPasswordContent button:has(.glyphicon-ban-circle)', function (e) {
            var current_target = $(e.currentTarget);
            var action = current_target.closest('form').attr('action');
            current_target.closest('form').replaceWith('<a id="userPasswordEditLink" href="' + action + '">Изменить пароль</a>');
        });

        $("#userDetails_roles_content").on('click', 'input[type="checkbox"]', function (e) {
            $("#userDetails_roles_content button").removeAttr('disabled');
        });

        $("#userDetails_roles_content button").click(function (e) {
            var form = $("#userDetails_RolesForm");
            var action = form.attr("action");
            var form_data = form.serialize();
            $.post(action, form_data, function (res) {
                if (res) {
                    $(e.currentTarget).attr("disabled", "disabled");
                }
                else {
                    alert("Сбой в запросе");
                    console.log('Сбой в запросе назначения ролей');
                }
            });
        });

        $('a[data-toggle="tab"]').mouseenter(function (e) {
            var current_tab = $(e.currentTarget);
            var tab_content = $(current_tab.attr('href'));
            if ($("a").is(tab_content.children())) {
                var action = $('a', tab_content).attr('href');;
                $.get(action, function (res) {
                    tab_content.html(res);
                    $('table[data-toggle="table"]', tab_content).bootstrapTable();
                });
            }
        });

        $('#apiKeysJoin').on('click', '#clientListModal button:has(.glyphicon-ok)', function (e) {
            var selected_client = $("#clientListModal table").bootstrapTable('getSelections');
            var date = $("#clientListModal #date").val();
            if (date === null) {
                $("#ExperienceTimeSpan").text('Дата действия должна быть выбрана');
            }
            if (selected_client.length > 0 && date !== null) {
                $("#ExperienceTimeSpan").text('');
                var form = $("#apiKeyAddForm");
                var client_id = selected_client[0].id;
                $("#apiKeyAddForm #ClientId").val(client_id);
                $("#apiKeyAddForm #ExperienceTime").val(date);
                $.post(form.attr('action'), form.serialize(), function (res) {
                    form.replaceWith(res);
                    $("#apiKeyDataTable").bootstrapTable('refresh');
                    $("#clientListModal").modal('hide');
                    $("#clientListModal table").bootstrapTable('refresh');
                });
            }
        });
    }
});

function userListActivatedFormatter(value) {
    var checkbox = '<input type="checkbox" name="activated_checbox"' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
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

function usersSelectActionFormatter(value, row) {
    return [
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function userList_response(res) {
    $.each(res.rows, function (index, element) {
        if (userSelected.indexOf(element.id) !== -1) {
            res.rows[index].state = true;
        }
    });
    return res;
}

function apiKeyListActionFormatter(value) {
    return [
        '<a class="update" href="javascript:void(0)" title="Детальная информация">',
        '<i class="glyphicon glyphicon-refresh" title="Обновить"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}