@{
    ViewBag.Title = "{{ModelName}}";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<div class="wrapper wrapper-content animated fadeInRight">
    <div class="ibox float-e-margins">
        <div class="ibox-content">
            <div class="row row-lg">
                <div class="col-sm-12">
                    <div>
                        <div>
                            <div class="btn-group hidden-xs" id="{{ModelClassName}}EventsToolbar" role="group">
								<!--<input placeholder="输入openID" class="ibox-input" v-on:keyup.enter="_FreshTable" id="openID" style="margin-left: 10px;">-->
                                <button type="button" action-id="{{ModelClassName}}-add" action-name="添加{{ModelName}}" class="btn btn-w-m btn-success authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Add">添加{{ModelName}}</button>
                                <button type="button" action-id="{{ModelClassName}}-update" action-name="修改{{ModelName}}" class="btn btn-w-m btn-primary authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Update">修改{{ModelName}}</button>
                                <button type="button" action-id="{{ModelClassName}}-delete" action-name="删除{{ModelName}}" class="btn btn-w-m btn-danger authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Delete">删除{{ModelName}}</button>
                            </div>
                            <table id="{{ModelClassName}}Table" data-side-pagination="server" data-sort-order="desc" data-mobile-responsive="true"></table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



<div class="modal inmodal" id="myModal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 v-if="add" class="modal-title">添加{{ModelName}}</h4>
                <h1 v-else class="modal-title" style="color: red">修改{{ModelName}}</h1>
            </div>
            <div class="modal-body">
			{% for field in ModelFields %}
				{% if field.Name == 'IsActive' %}
			<div class="form-group">
				<label class="control-label">{{field.Comment}}：&nbsp;&nbsp;&nbsp;</label>
				<input v-model="currentRow.{{field.Name}}" class="js-switch" type="checkbox" id="{{field.Name}}" />&nbsp;&nbsp;{{currentRow.{{field.Name}} ? "是" : "否"}}
			</div>
				{% else %}
			<div class="form-group">
				<label class="control-label">{{field.Comment}}：</label>
				<input v-model="currentRow.{{field.Name}}" class="form-control" type="text" placeholder="请输入{{field.Comment}}">
			</div>
				{% endif %}
			{% endfor %}
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-white" id="close-model" v-on:click="_Close">关闭</button>
                <button type="button" class="btn btn-primary" id="save-model" v-on:click="_Save">提交</button>
            </div>
        </div>
    </div>
</div>
 
 @section Scripts{ 
		<script type="text/javascript">

		var vm = new Vue({
			el: 'body',
			data: {
				add: true,
				currentRow: {}
			},
			computed: {},
			ready:function(){
				auth();
				//initSwitchery();
				OnReady();
			},
			methods: {

				_{{ModelClassName}}Add: function () {
					vm.add = true;
					$('#myModal').modal({ backdrop: 'static', keyboard: false });
					$('#myModal').modal('show');
				},
				_FreshTable: function () {
					try {
						$(".bootstrap-table button[name='refresh']")[0].click();
					} catch (e) {

					}
				},
				_Close: function () {
					vm.currentRow = {};
					$('#myModal').modal('hide');
				},
				_Save: function () {
					//if (!vm.currentRow.Value) {
					//    swal({
					//        title: "",
					//        text: "请输入描述！",
					//        type: "error"
					//    });
					//    return;
					//}
               
					QQT.ajax('/{{ModelClassName}}/Add{{ModelClassName}}', 'Post', vm.currentRow)
						.done(function (response) {
							$('#{{ModelClassName}}Table').bootstrapTable('refresh', { silent: true });
							swal("成功啦！", "操作成功!", "success");
							vm._Close();
						});
				},
				_{{ModelClassName}}Update: function () {
					vm.add = false;
					var selectRow = $('#{{ModelClassName}}Table').bootstrapTable('getSelections');
					if (selectRow.length < 1) {
						swal({
							title: "",
							text: "请选择！",
							type: "error"
						});
						return;
					}
					vm.currentRow = selectRow[0];
					$('#myModal').modal({ backdrop: 'static', keyboard: false });
					$('#myModal').modal('show');
				},
				_{{ModelClassName}}Delete: function () {
					var selectRow = $('#{{ModelClassName}}Table').bootstrapTable('getSelections');
					if (selectRow.length < 1) {
						swal({
							title: "",
							text: "请选择！",
							type: "error"
						});
						return;
					}
					var tid = selectRow[0].Tid;
					swal({
							title: "您确定要删除吗?",
							text: "请谨慎操作！",
							type: "warning",
							showCancelButton: true,
							confirmButtonColor: "#DD6B55",
							confirmButtonText: "是的，朕决心已定！",
							cancelButtonText: "让朕再考虑一下…",
							showLoaderOnConfirm: true,
							preConfirm: function () {
								return QQT.ajax('/{{ModelClassName}}/Del{{ModelClassName}}','Post',{tid:tid});                
							}
                    }).then(function (response) {
                        $('#{{ModelClassName}}Table').bootstrapTable('refresh', { silent: true });
                        swal("成功啦！", "操作成功!", "success");
                    }).catch(function() {
                    
                    });
				}
			}
		});

		function OnReady(){
			$('#{{ModelClassName}}Table').bootstrapTable({
				ajax: ajaxRequest,
				pageNumber: 1,
				pageSize: 10,
				pageList: [10, 25, 50, 100],
				resetOffset: true,
				search: false,
				sortable: true,
				pagination: true,
				height: $(window).height(),
				showRefresh: true,
				showToggle: true,
				showColumns: false,
				striped: true,
				sortOrder: 'desc',
				clickToSelect: true,
				singleSelect: true,
				cache: false,
				// showPaginationSwitch:true,
				dataType: "json",
				iconSize: 'outline',
				toolbar: '#{{ModelClassName}}EventsToolbar',
				icons: {
					refresh: 'glyphicon-repeat',
					toggle: 'glyphicon-list-alt',
					columns: 'glyphicon-list'
				},
				columns: [
					{
						field: 'state',
						title: '',
						checkbox: true
					},
				{% for field in ModelFields %}
					{
						field: '{{field.Name}}',
						title: '{{field.Comment}}',
						sortable: true
					},
				{% endfor %}
                   
				],
				onLoadSuccess: function () {
					tipReCreate();
				},
				onToggle: function () {
					tipReCreate();
				},
				onEditableSave: function (field, row, oldValue, $el) {

				},
				rowStyle: function (row, index) {
					//这里有5个取值代表5中颜色['active', 'success', 'info', 'warning', 'danger'];
					var strclass = "";
					return {};
					//return { classes: strclass }
				}
			});

			$(window).resize(function () {
				$('#{{ModelClassName}}Table').bootstrapTable('resetView', { height: $(window).height() });
				tipReCreate();
			});
		}

		function tipReCreate() {
			setTimeout(function () {
				Tipped.create('.tip_infomation',
					function (element) {
						return "<strong>" + $(element).data('content') + "</strong>";
					});
			}, 200);
		}

		function LongStringFormatter(value, row, index) {
			if (value.length <= 15) {
				return value;
			}
			else {
				return "<div class='tip_infomation' data-content='" + QQT.InsertEnter(value, 10) + "'>" + value.substring(0, 15) + "...</div>";
			}
		}


		function ajaxRequest(params) {
			var pageSize = params.data.limit;
			var pageIndex = params.data.offset / params.data.limit + 1;
			var orderBy = params.data.sort;
			var orderSequence = params.data.order;
			QQT.ajax('/{{ModelClassName}}/Get{{ModelClassName}}List',
				'POST',
				{
					pageIndex: pageIndex,
					pageSize: pageSize,
					orderBy: orderBy,
					orderSequence: orderSequence
				})
			.done(function (response) {
				params.success({
					total: response.Total,
					rows: response.Rows
				});
			});
		}


	</script>
}