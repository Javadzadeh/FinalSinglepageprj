// Form
var form = document.getElementById("form");

// Buttons
var btnInsert = document.getElementById("btnInsert");
var btnEdit = document.getElementById("btnEdit");
var btnDeleteAll = document.getElementById("btnDeleteAll");
var btnRefresh = document.getElementById("btnRefresh");
var btnDeleteConfirm = document.getElementById("btnDeleteConfirm");

// Inputs
var id = document.getElementById("id");
var firstName = document.getElementById("firstName");
var lastName = document.getElementById("lastName");
var email = document.getElementById("email");

// Validation Messages
var firstNameValidationMessage = document.getElementById("firstNameValidationMessage");
var lastNameValidationMessage = document.getElementById("lastNameValidationMessage");
var emailValidationMessage = document.getElementById("emailValidationMessage");

// Delete Modal
var deleteConfirmModal = document.getElementById("deleteConfirmModal");
var inDeleteConfirmModalId = document.getElementById("inDeleteConfirmModalId");
var deleteConfirmModalBody = document.getElementById("deleteConfirmModalBody");

// Details Modal
var detailsModal = document.getElementById("detailsModal");

// Others
var chkSelectAll = document.getElementById("selectAll");
var tbody = document.querySelector("tbody");
var resultMessage = document.getElementById("resultMessage");

// Event Listeners
form.addEventListener("submit", Add);
btnRefresh.addEventListener("click", LoadData);
btnEdit.addEventListener("click", Edit);
chkSelectAll.addEventListener("click", ToggleSelectAll);
detailsModal.addEventListener("show.bs.modal", Details);
chkSelectAll.addEventListener("change", DeselectAll);
deleteConfirmModal.addEventListener("show.bs.modal", ConfirmDelete);

// Functionality
var selectedRows = [];
var allRowsCount;

window.onload = LoadData();



async function LoadData() {
    try {
        console.log("start loading");
        RefreshForm();
        chkSelectAll.checked = false;
        DeselectAll();
        btnDeleteAll.disabled = true;  // غیرفعال کردن دکمه Delete All در ابتدا
        tbody.innerHTML = "";

        const res = await fetch("http://localhost:5268/Person/GetAll");
        const dto = await res.json();
        allRowsCount = dto.length;

        let html = "";
        dto.forEach(function (person) {
            html += `
                <tr id="${person.id}">
                    <td>
                        <input id="${person.id}" class="form-check-input selectRow" type="checkbox" ${selectedRows.includes(person.id) ? "checked" : ""} />
                    </td>
                    <td id="firstName">${person.firstName}</td>
                    <td id="lastName">${person.lastName}</td>
                    <td id="email">${person.email}</td>
                    <td>
                        <input id="${person.id}" class="btn btn-outline-primary btn-sm" type="button" value="Details" data-bs-toggle="modal" data-bs-target="#detailsModal">
                        <input id="${person.id}" class="btn btn-outline-danger btn-sm" type="button" value="Delete" data-bs-toggle="modal" data-bs-target="#deleteConfirmModal">
                    </td>
                </tr>`;
        });
        tbody.innerHTML = html;

        // Add event listener for checkboxes after loading the data
        const checkboxes = tbody.querySelectorAll("input[type='checkbox']");
        checkboxes.forEach(function (checkbox) {
            checkbox.addEventListener('click', function () {
                SelectRow(this);
            });
        });
    } catch (error) {
        console.error("Error loading data:", error);
    }
}




function SelectRow(checkBox) {
    if (checkBox.checked) {
        // اضافه کردن آی دی چک‌باکس به selectedRows
        if (!selectedRows.includes(checkBox.id)) {
            selectedRows.push(checkBox.id);
        }
    } else {
        // حذف آی دی از selectedRows
        selectedRows = selectedRows.filter((id) => id !== checkBox.id);
    }
    UpdateUI();  // به روز رسانی UI با اطلاعات جدید
}




function UpdateUI() {
    console.clear(); // برای حذف لاگ‌های قبلی
    if (selectedRows.length === 1) {
        // فقط زمانی که یک سطر انتخاب شده
        RefreshForm();  // فرم رو برای به روز رسانی آماده می‌کنیم
        btnEdit.disabled = false;
        btnDeleteAll.disabled = false;
        id.value = selectedRows[0];

        // به روز رسانی فیلدهای فرم از اطلاعات سطر انتخاب شده
        let selectedRow = document.querySelector(`tr[id="${selectedRows[0]}"]`);
        firstName.value = selectedRow.querySelector("td[id='firstName']").innerText;
        lastName.value = selectedRow.querySelector("td[id='lastName']").innerText;
        email.value = selectedRow.querySelector("td[id='email']").innerText;
    } else if (selectedRows.length > 1) {
        // اگر بیش از یک سطر انتخاب شده باشه
        RefreshForm();
    } else {
        // اگر هیچ سطری انتخاب نشده باشه
        RefreshForm();
        btnEdit.disabled = true;
        btnDeleteAll.disabled = true;
    }
}







function ToggleSelectAll() {
    const checkboxes = tbody.querySelectorAll("input[type='checkbox']");
    selectedRows = [];  // بازنشانی لیست انتخاب‌ها

    checkboxes.forEach(function (checkbox) {
        checkbox.checked = chkSelectAll.checked;  // تنظیم وضعیت چک‌باکس‌ها بر اساس وضعیت "Select All"

        if (checkbox.checked) {
            selectedRows.push(checkbox.id);  // اگر تیک خورده بود، به لیست انتخاب‌ها اضافه می‌شود
        }
    });

    // بررسی اینکه آیا همه سطرها انتخاب شده‌اند یا نه
    if (selectedRows.length === allRowsCount) {
        btnDeleteAll.disabled = false;  // اگر همه سطرها انتخاب شده باشند، دکمه Delete All فعال می‌شود
    } else {
        btnDeleteAll.disabled = true;  // در غیر این صورت، دکمه Delete All غیرفعال می‌شود
    }

    UpdateUI();
}






function Add(e) {
    e.preventDefault();
    let isValidData = ValidateFormData();
    if (isValidData) {
        let dto = {
            Id: "",
            FirstName: firstName.value,
            LastName: lastName.value,
            Email: email.value
        };
        fetch("http://localhost:5268/Person/Post", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Accept: "*/*"
            },
            body: JSON.stringify(dto)
        }).then((res) => {
            if (res.status === 409) {
                emailValidationMessage.innerText = `Person with Email : ${email.value} already exists`;
                email.classList.add("is-invalid");
            } else if (res.status === 200) {
                TriggerResultMessage("Operation Successful");
                LoadData();
            } else {
                TriggerResultMessage("Operation Failed");
            }
        });
    }
}




function RefreshForm(clearValues = true) {
    firstName.classList.remove("is-invalid", "is-valid");
    lastName.classList.remove("is-invalid", "is-valid");
    email.classList.remove("is-invalid", "is-valid");

    if (clearValues) {
        firstName.value = "";
        lastName.value = "";
        email.value = "";
    }

    firstNameValidationMessage.innerText = "";
    lastNameValidationMessage.innerText = "";
    emailValidationMessage.innerText = "";
}


function ValidateFormData() {
    let isValidData = true;

    if (firstName.value === "") {
        firstNameValidationMessage.innerText = "First name is required";
        firstName.classList.add("is-invalid");
        isValidData = false;
    } else {
        firstName.classList.add("is-valid");
    }

    if (lastName.value === "") {
        lastNameValidationMessage.innerText = "Last name is required";
        lastName.classList.add("is-invalid");
        isValidData = false;
    } else {
        lastName.classList.add("is-valid");
    }

    if (!/^([a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$/.test(email.value)) {
        emailValidationMessage.innerText = "Invalid email format";
        email.classList.add("is-invalid");
        isValidData = false;
    } else {
        email.classList.add("is-valid");
    }

    return isValidData;
}

function TriggerResultMessage(message) {
    resultMessage.innerText = message;
    resultMessage.style.opacity = "1";
    setTimeout(function () {
        resultMessage.style.opacity = "0";
    }, 2000);
}



function Edit() {
    RefreshForm(false);
    let isValidData = ValidateFormData();

    if (isValidData) {
        let updateDto = {
            Id: id.value,
            FirstName: firstName.value,
            LastName: lastName.value,
            Email: email.value,
        };

        fetch("http://localhost:5268/Person/Put", {

            method: "Post",
            headers: {
                "Content-Type": "application/json",
                Accept: "*/*",
            },
            body: JSON.stringify(updateDto),
        }).then((res) => {
            console.log("test");
            if (res.status == 409) {
                vmNationalCode.innerText = `Person with NationalCode : ${iptNationalCode.value} already exist`;
                iptNationalCode.classList.add("is-invalid");
            } else if (res.status == 200) {
                TriggerResultMessage("Operation Successful");
                LoadData();
            } else {
                TriggerResultMessage("Operation Failed");
            }
        });
    }
}





function Details(event) {
    // متغیرهای ورودی برای دریافت مقادیر از فرم
    let updateDto = {
        Id: id.value,               // اینجا باید id را درست از DOM بگیرید
        FirstName: firstName.value, // و همینطور firstName و دیگر مقادیر
        LastName: lastName.value,
        Email: email.value,
    };

    // ارسال درخواست به سرور
    fetch("http://localhost:5268/Person/Detail", {
        method: "PUT",  // یا 'PUT' بسته به نیاز شما
        headers: {
            "Content-Type": "application/json",  // هدر برای JSON
        },
        body: JSON.stringify(updateDto),  // ارسال داده‌ها به صورت JSON
    })
        .then((res) => res.json())
        .then((json) => {
            // پردازش پاسخ سرور
            let inModalUl = document.querySelectorAll(".card li");
            inModalUl[0].innerText = `First Name : ${json.firstName}`;
            inModalUl[1].innerText = `Last Name : ${json.lastName}`;
            inModalUl[2].innerText = `Email : ${json.email}`;
        })
        .catch((error) => {
            console.error("Error:", error);
        });
}




function ConfirmDelete(event) {
    let clickedButton = event.relatedTarget; // دکمه‌ای که مدال رو باز کرده
    var idsToDelete = selectedRows;
    if (clickedButton.value == "Delete") {
        
        let updateDto = {
            Id: id.value,               // اینجا باید id را درست از DOM بگیرید
          
        };

        inDeleteConfirmModalId.value = updateDto.Id; // انتقال ID به فیلد مدال
        PassDetailsToDeleteConfirm(updateDto.Id); // انتقال اطلاعات شخص به مدال

        btnDeleteConfirm.addEventListener("click", Delete); // حذف یک رکورد
        
    }
    else {
        if (idsToDelete.length == 1) {
            PassDetailsToDeleteConfirm(selectedRowsList[0]); // در صورتی که فقط یک رکورد انتخاب شده
        }
        else if (clickedButton.value == "DeleteAll") {
           
            deleteConfirmModalBody.innerHTML = `You are deleting <strong>${idsToDelete.length} records</strong>, Are you sure?`;
            btnDeleteConfirm.addEventListener("click", deleteSelectedRows);
        }


    }
   
    
}


function PassDetailsToDeleteConfirm(id) {
    let firstName = document.querySelector(
        `tr[id="${id}"] td[id="firstName"]`
    ).innerText;
    let lastName = document.querySelector(
        `tr[id="${id}"] td[id="lastName"]`
    ).innerText;
    let Email = document.querySelector(
        `tr[id="${id}"] td[id="email"]`
    ).innerText;
    deleteConfirmModalBody.innerHTML = `You are deleting :<br><strong>First Name : ${firstName}<br>Last Name : ${lastName}<br> Email : ${Email}</strong><br>Are you sure ?`;
}




function Delete(event) {
    let id = inDeleteConfirmModalId.value;

    fetch("http://localhost:5268/Person/Delete", {
        method: "Delete", // تغییر به DELETE
        headers: {
            "Content-Type": "application/json",
            Accept: "*/*",
        },
        body: JSON.stringify({ Id: id }),
    }).then((res) => {
        if (res.ok) { // استفاده از res.ok برای بررسی وضعیت موفقیت‌آمیز
            TriggerResultMessage("Operation Successful");
            // حذف سطر از selectedRows بعد از موفقیت‌آمیز بودن حذف
            selectedRows = selectedRows.filter(item => item !== id);
            LoadData();  // دوباره داده‌ها را بارگذاری کن
        } else {
            TriggerResultMessage("Operation Failed");
        }
    }).catch((error) => {
        TriggerResultMessage("An error occurred: " + error.message);
    });

    inDeleteConfirmModalId.value = "";
}







function DeselectAll() {
    // برای هر چک‌باکس که کلاس آن selectRow است، وضعیت آن را به روز می‌کنیم
    document.querySelectorAll(".selectRow").forEach((checkBox) => {
        checkBox.checked = false;  // همه چک‌باکس‌ها را غیرفعال می‌کنیم
    });

    // بررسی وضعیت تیک چک‌باکس "select all"
    if (chkSelectAll.checked) {
        // اگر چک‌باکس "select all" فعال بود
        document.querySelectorAll(".selectRow").forEach((checkBox) => {
            checkBox.checked = true;  // همه چک‌باکس‌ها را فعال می‌کنیم
            if (!selectedRows.includes(checkBox.id)) {
                selectedRows.push(checkBox.id);  // اضافه کردن به لیست انتخاب‌ها
            }
        });
    } else {
        selectedRows = [];  // در صورتی که "select all" غیرفعال شده باشد، تمام انتخاب‌ها رو حذف می‌کنیم
    }
}




// درخواست حذف داده‌ها از دیتابیس
function deleteSelectedRows() {
    var idsToDelete = selectedRows;

    if (idsToDelete.length > 0) {
        fetch("http://localhost:5268/Person/DeleteAll", {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ ids: idsToDelete }),
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error('Failed to delete rows');
            })
            .then(data => {
                console.log('Rows deleted successfully:', data);
                selectedRows = [];
                var deleteModalInstance = bootstrap.Modal.getInstance(deleteConfirmModal);
                if (deleteModalInstance) {
                    deleteModalInstance.hide();
                }
                LoadData();
            })
            .catch(error => console.error('Error deleting rows:', error));
    } else {
        alert('لطفاً سطرهایی را برای حذف انتخاب کنید.');
    }
    RefreshForm();
}

