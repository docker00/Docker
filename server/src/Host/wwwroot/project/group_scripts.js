var groupId = "";
var selectedUsers = [];
var selectedRoles = [];

$(function () {
    'use strict';

    $("#groupList_content")
        .on('click', 'a.glyphicon-trash', function (e) {
            e.preventDefault();
            e.stopPropagation();
            var current_target = $(e.currentTarget);
            if (confirm("Вы действительно хотите " + $(e.currentTarget).attr("title"))) {
                var action = current_target.attr("href");
                if (action.split('/')[2] === "Delete") {
                    var deleteChildren = confirm('Удалить дочерние группы?');
                    action += "&deleteChilden=" + deleteChildren;
                }
                $.get(action, function (res) {
                    if (res === true) {
                        var group_id = current_target.attr('href').split('=')[1];
                        current_target.closest('li').remove();
                        $("h4,h3", "ul").each(function (index, e) {
                            if (e.id === "listGroup_group_" + group_id) {
                                $(e).closest('li').remove();
                            }
                        });
                    }
                });
            }
        })
        .on('click', 'a.glyphicon-remove', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (confirm("Вы действительно хотите " + $(e.currentTarget).attr("title"))) {
                var current_target = $(e.currentTarget);
                var action = current_target.attr('href');
                $.get(action, function (res) {
                    if (res !== false) {
                        current_target.closest('li').remove();
                    }
                });
            }
        })
        .on('click', 'a[name="groupTreeParents"]', function (e) {
            e.preventDefault();
            e.stopPropagation();
            var current_target = $(e.currentTarget);
            var closest_li = current_target.closest('li');
            closest_li.find('ul').remove();
            if ($(closest_li).find(' > h3,h4 > a > span').attr('class') === 'glyphicon glyphicon-minus') {
                $(closest_li).find(' > h3,h4 > a > span').attr('class', 'glyphicon glyphicon-plus');
            } else {
                $.get(current_target.attr('href'), function (res) {
                    if (res) {
                        $(closest_li).find(' > h3,h4 > a > span').attr('class', 'glyphicon glyphicon-minus');
                        $(closest_li).append(res);
                        $(closest_li.find('table')).bootstrapTable();
                    }
                });
            }
        })
        .on('click', 'form[name="addForms"] button:has(.glyphicon-ban-circle)', function (e) {
            var form = $(e.currentTarget).closest('form');
            form.closest('li').remove();
        })
        .on('click', 'a[name="addBtns"]', function (e) {
            e.preventDefault();
            e.stopPropagation();
            var current_target = $(e.currentTarget);
            if (current_target.closest('li').prev().find('form').length === 0) {
                $.get(current_target.attr('href'), function (res) {
                    current_target.closest('li').before('<li>' + res + '</li>');
                });
            }
        })
        .on('click', 'button[name="UserSelectBtns"]', function (e) {
            var current_target = $(e.currentTarget);
            groupId = current_target.val();
            var selectedUsers = [];
            $("#userSelectModal").modal('show');
            $("#userSelectModal table").bootstrapTable('refresh');
        })
        .on('click', 'button[name="RoleSelectBtns"]', function (e) {
            var current_target = $(e.currentTarget);
            groupId = current_target.val();
            var selectedRoles = [];
            $("#roleSelectModal").modal('show');
            $("#roleSelectModal table").bootstrapTable('refresh');
        });

    $("#userSelectModal button:has(.glyphicon-ok)").click(function (e) {
        if (selectedUsers.length > 0) {
            var user_ids = $.map(selectedUsers, function (user, index) {
                return user.id;
            });
            var data = { groupId: groupId, users_ids: user_ids };
            $.post("/Group/UserAdd", data, function (res) {
                if (res === true) {
                    selectedUsers = [];
                    var id = "#userList_toolbar_" + groupId;
                    $(id).closest('.bootstrap-table').find('table').bootstrapTable('refresh');
                    $("#userSelectModal").modal('hide');
                }
            });
        }
    });

    $('#usersList_data_table').on('check.bs.table uncheck.bs.table ' +
        'check-all.bs.table uncheck-all.bs.table', function (e, rows) {
            if ($.isArray(rows)) {
                if (e.type === "uncheck-all") {
                    var selected_ids = $.map(rows, function (user, index) {
                        return user.id;
                    });
                    selectedUsers = $.map(selectedUsers, function (user, index) {
                        if ($.inArray(user.id, selected_ids) === -1) {
                            return user;
                        }
                    });
                }
                else {
                    $.each(rows, function (index, user) {
                        selectedUsers.push(user);
                    });
                }
            }
            else {
                var deleted = false;
                selectedUsers = $.map(selectedUsers, function (user, index) {
                    if (user.id !== rows.id) {
                        return user;
                    }
                    else {
                        deleted = true;
                    }
                });
                if (!deleted) {
                    selectedUsers.push(rows);
                }
            }
        });

    $("#roleSelectModal button:has(.glyphicon-ok)").click(function (e) {
        if (selectedRoles.length > 0) {
            var role_ids = $.map(selectedRoles, function (role, index) {
                return role.id;
            });
            var data = { groupId: groupId, rolesIds: role_ids };
            $.post("/Group/RoleAdd", data, function (res) {
                if (res === true) {
                    selectedRoles = [];
                    var id = "#roleList_toolbar_" + groupId;
                    $(id).closest('.bootstrap-table').find('table').bootstrapTable('refresh');
                    $("#roleSelectModal").modal('hide');
                }
            });
        }
    });

    $('#roleList_data_table').on('check.bs.table uncheck.bs.table ' +
        'check-all.bs.table uncheck-all.bs.table', function (e, rows) {
            if ($.isArray(rows)) {
                if (e.type === "uncheck-all") {
                    var selected_ids = $.map(rows, function (role, index) {
                        return role.id;
                    });
                    selectedRoles = $.map(selectedRoles, function (role, index) {
                        if ($.inArray(role.id, selected_ids) === -1) {
                            return role;
                        }
                    });
                }
                else {
                    $.each(rows, function (index, role) {
                        selectedRoles.push(role);
                    });
                }
            }
            else {
                var deleted = false;
                selectedRoles = $.map(selectedRoles, function (role, index) {
                    if (role.id !== rows.id) {
                        return role;
                    }
                    else {
                        deleted = true;
                    }
                });
                if (!deleted) {
                    selectedRoles.push(rows);
                }
            }
        });

    window.groupRoleListOperateEvents = {
        'click .details': function (e, value, row, index) {
            window.location.href = '/Role/Details/?roleId=' + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить роль " + row.name + " из группы?")) {
                var groupId = $(e.currentTarget).closest('table').attr('id').split('_')[1];
                var fromUsersDelete = confirm('Хотите удалить эту роль у пользователей группы?');
                var data = { roleId: row.id, groupId: groupId, fromUsersDelete: fromUsersDelete };
                $.get('/Group/RoleDelete', data, function (res) {
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
    window.groupUserListOperateEvents = {
        'click .details': function (e, value, row, index) {
            window.location.href = '/User/Details/?userId=' + row.id;
        },
        'click .remove': function (e, value, row, index) {
            if (confirm("Вы действительно хотите удалить пользователя " + row.userName + " из группы?")) {
                var groupId = $(e.currentTarget).closest('table').attr('id').split('_')[1];
                var data = { userId: row.id, groupId: groupId };
                $.get('/Group/RemoveUser', data, function (res) {
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

function userResposeHandler(res) {
    var selected_ids = $.map(selectedUsers, function (user, index) {
        return user.id;
    });
    $.each(res.rows, function (index, element) {
        if (selected_ids.indexOf(element.id) !== -1) {
            res.rows[index].check = true;
        }
    });
    return res;
}
function userQueryFormatter(params) {
    if (groupId !== "") {
        params.groupId = groupId;
    }
    return params;
}

function roleResposeHandler(res) {
    var selected_ids = $.map(selectedRoles, function (role, index) {
        return role.id;
    });
    $.each(res.rows, function (index, element) {
        if (selected_ids.indexOf(element.id) !== -1) {
            res.rows[index].check = true;
        }
    });
    return res;
}
