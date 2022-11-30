﻿//=======================
// INITIALIZE DATA
//=======================
var cari = '';
let pageProduct = {};
pageProduct.page = 1;
pageProduct.hasContent = false;
pageProduct.hasNext = false;
pageProduct.hasPrevious = false;
let api = "/Master/Product";
let product = {};
let txtGUID = $("#txtGUID").val()
let __RequestVerificationToken = $('#frm input[name=__RequestVerificationToken]').val()
let headerData = {};
headerData.__RequestVerificationToken = __RequestVerificationToken;
let elementToSwitch = [$("#productFormRow"), $("#productTableRow")];
let $productFormTitle = $("#productFormTitle");

//=======================
// ALL BUTTON
//=======================
let $btnPrev = $("#btnPrev");
let $btnNext = $("#btnNext");
let $btnSave = $("#btnSave");
let $btnAddProduct = $("#btnAddProduct");
let $btnCancelAddProduct = $("#btnCancelAddProduct");
//=======================
// FORM 
//=======================
let $txtProductCode = $("#txtProductCode");
let $txtProductDescription = $("#txtProductDescription");
let $intLevel = $("#intLevel");
let $ImportExcel = $("#ImportExcel");

function initializePage() {
    page = 1;
    cari = "";
    pageProduct = {};
    pageProduct.page = 1;
    pageProduct.hasContent = false;
    pageProduct.hasNext = false;
    pageProduct.hasPrevious = false;
    api = "/Master/Product";
    user = {};
    txtGUID = $("#txtGUID").val()
    __RequestVerificationToken = $('#frm input[name=__RequestVerificationToken]').val()
    headerData = {};
    headerData.__RequestVerificationToken = __RequestVerificationToken;
    setButtonNextPrvVisibility();
    iniitializeData();
}
initializePage();
getAllProduct(pageProduct.page, cari);
function setButtonNextPrvVisibility() {
    if (pageProduct.hasNext) {
        $btnNext.show();
    } else {
        $btnNext.hide();
    }
    if (pageProduct.hasPrevious) {
        $btnPrev.show()
    } else {
        $btnPrev.hide();
    }
}


function errorHandle(error) {
    $.errorMessage("Error", error.responseJSON.txtMessage)
}

function iniitializeData() {
    $.postApi(api + "/InitiateData", null, function (response) {
        product = response.objData;
        setupAddProductForm();
        $productFormTitle.text = "Add Product";
    }, errorHandle, headerData);
}

function setupAddProductForm() {
    $txtProductCode.val(product.txtProductCode);
    $txtProductDescription.val(product.txtProductDescription);
    $intLevel.val(product.intLevel);
}

//=======================
// AJAX REQUEST GET ALL PRODUCT
//=======================
function getAllProduct(x, y) {
    $.postApi(api + "/getAllProduct",
        {
            page: x,
            cari: y,
            size: 20
        },
        function (response) {
            pageProduct = response.objData;
            fillTableData();
        },
        function (e) {
            $.errorMessage("Failed", e.responseJSON.txtMessage);
            $("#tbody").html("");
            $("#tbody").append("<tr><td colspan='5'><center><span class='text-muted'>No Data <a class='refresh' href=''>Refresh</a> </span></center></td></tr>");
        },
        headerData
    )
}
//=======================
// AJAX REQUEST SAVE PRODUCT
//=======================
function saveProduct() {
    $.postApi(api + "/Save",
        product,
        function (response) {
            iniitializeData();
            $.switchElement(elementToSwitch, 1);
            $.successMessage("Sukses", "Berhasil menyimpan product");
            getAllProduct(0, cari)
        },
        function (e) {
            $.errorMessage("Failed", e.responseJSON.txtMessage);
            $("#tbody").html("");
            $("#tbody").append("<tr><td colspan='5'><center><span class='text-muted'>No Data <a class='refresh' href=''>Refresh</a> </span></center></td></tr>");
        },
        headerData
    )
}
//=======================
// FILL TABLE DATA
//=======================
function fillTableData() {
    $("#tbody").html("");
    if (pageProduct.hasContent) {
        for (let i = 0; i < pageProduct.content.length; i++) {
            var m = pageProduct.content[i];
            let $btnEdit = `<button class=' btn btn-outline-warning btnEdit ' data-product='` + JSON.stringify(m) + `'> <i class='fa fa-edit'></i> Edit</button>`;
            let row = "<tr id='intProductID" + m.intProductID + "'>" +
                "<td class='text-center'>" + m.intProductID + "</td>" +
                "<td class='text-center'>" + m.txtProductCode + "</td>" +
                "<td class='text-center'>" + m.txtProductDescription + "</td>" +
                "<td class='text-center'>" + m.intLevel + "</td>" +
                "<td class='text-center'>" +
                $btnEdit +
                "</td>" +
                "</tr>";
            $("#tbody").append(row);
        }
        $(".btnEdit").on('click', function () {
            let r = $(this).data('product');
            setupDataProductEdit(r);
            setupDataProductEditForm();
            $.switchElement(elementToSwitch, 0);
            $productFormTitle.text = "Edit Products";
        })

    } else {
        $("#tbody").append("<tr><td colspan='5'><center><span class='text-muted'>No Data <a class='refresh' href=''>Refresh</a> </span></center></td></tr>");
    }
    setButtonNextPrvVisibility();
}
//=======================
// ASSIGN PRODUCT SAVE
//=======================
function setupDataProduct() {
    product.txtProductCode = $txtProductCode.val().toString();
    product.txtProductDescription = $txtProductDescription.val().toString();
    product.intLevel = parseInt($intLevel.val().toString());
}
//=======================
// ASSIGN EDIT PRODUCT
//=======================
function setupDataProductEdit(r) {
    product.intProductID = r.intProductID;
    product.txtProductCode = r.txtProductCode;
    product.txtProductDescription = r.txtProductDescription;
    product.intLevel = r.intLevel;
    product.txtGUID = r.txtGUID;
}
//=======================
// ASSIGN EDIT PRODUCT FORM
//=======================
function setupDataProductEditForm() {
    $txtProductCode.val(product.txtProductCode);
    $txtProductDescription.val(product.txtProductDescription);
    $intLevel.val(product.intLevel)
}

//=======================
// EXPORT EXCEL
//=======================
function ExportExcelHendler(d) {

    let wb = XLSX.utils.book_new();
    let data = d
    wb.Props = {
        Title: "Product E-RPS Export",
        Subject: "Product E-RPS",
        Author: "Application E-RPS",
        Company: "PT Sanghiang Perkasa",
        Category: "Report",
        CreatedDate: new Date()
    }
    wb.SheetNames.push("Export")

    var ws = XLSX.utils.json_to_sheet(data)
    wb.Sheets["Export"] = ws

    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    return wbout;
}

function s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
    return buf
}
//=======================
// IMPORT EXCEL
//=======================
function hendelFile(e) {
    let files = e.target.files;
    for (i = 0, f = files[i]; i != files.length; ++i) {
        var reader = new FileReader();
        var name = f.name;
        reader.onload = function (e) {
            var data = e.target.result;

            var result;
            try {
                var workbook = XLSX.read(data, { type: 'binary' });

                var sheet_name_list = workbook.SheetNames;
                sheet_name_list.forEach(function (y) { /* iterate through sheets */
                    //Convert the cell value to Json
                    var roa = XLSX.utils.sheet_to_json(workbook.Sheets[y]);
                    if (roa.length > 0) {
                        result = roa;
                    }
                });
                //Get the result                
                dataExcel = result

                $.postApi(api + "/ImportExcel",
                    { productRequest: dataExcel },
                    function (response) {
                        if (response.bitSuccess == true) {
                            $.successMessage('Excel', "Berhasil di Save")
                        } else {
                            $.errorMessage('Failed', "Gagal Memperoses Import")
                        }
                    },
                    function (e) {
                        $.errorMessage("Failed", e.responseJSON.txtMessage);
                    },
                    headerData
                )
            } catch (e) {
                $.errorMessage('Excel', "Tidak dapat memproses file !")
            }

        };
        reader.readAsArrayBuffer(f);
    }
}

//=======================
// EVENT LISTENER
//=======================
$btnNext.bind('click', () => {
    if (pageProduct.hasNext) {
        getAllProduct(pageProduct.page + 1, cari);
    }
})

$btnPrev.bind('click', () => {
    if (pageProduct.hasPrevious) {
        getAllProduct(pageProduct.page - 1, cari);
    }
})
$btnAddProduct.bind('click', () => {
    $.switchElement(elementToSwitch, 0);
    iniitializeData();
})
$btnCancelAddProduct.bind('click', () => {
    $.switchElement(elementToSwitch, 1);
    $productFormTitle.text = "Add Product";
})
$btnSave.bind('click', () => {
    setupDataProduct();
    $.confirmMessage("Confirm", "Save Product?", "Ya Save", saveProduct);
})

function searchProduct() {
    pageProduct.page = 1;
    getAllProduct(pageProduct.page, cari);
}

$("#ExportExcel").click(() => {
    $.postApi(api + "/ExportExcel",
        {

        },
        function (response) {
            let wbout = ExportExcelHendler(response.objData);

            saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), `Product Export - ${new Date()}.xlsx`)
        },
        function (e) {
            $.errorMessage("Failed", e.responseJSON.txtMessage);

        },
    )
})

$(document).ready(() => {
    $ImportExcel.change(hendelFile)
})
