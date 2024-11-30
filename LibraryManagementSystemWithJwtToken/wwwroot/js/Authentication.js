let isCanBeValidate = {
    Username: false,
    Password: false,
};

function fieldValidation(validationObject) {
    let isValidObject = true;

    for (const [key, value] of Object.entries(validationObject)) {
        const element = document.getElementById(key);
        const errorElement = document.getElementById(`${key}Error`);

        if (element) {
            const elementValue = element.value.trim();
            element.style.borderColor = "";
            if (errorElement) errorElement.style.display = "none";

            if (!value || elementValue === "") {
                isValidObject = false;
                element.style.borderColor = "red";
                if (errorElement) errorElement.style.display = "block";

                if (element.classList.contains("select2")) {
                    $(element).next('.select2').find('.select2-selection').css('border-color', 'red');
                }
            }
            const handleValidInput = () => {
                if (element.value.trim() !== "") {
                    validationObject[key] = true;
                    element.style.borderColor = "";
                    if (errorElement) errorElement.style.display = "none";
                }
            };


            element.addEventListener('change', handleValidInput);
        }
    }

    return isValidObject;
}
document.addEventListener("change", function () {
for (const [key, value] of Object.entries(isCanBeValidate)) {
const element = document.getElementById(key);

    if (element && element.value.trim() !== "") {
        isCanBeValidate[key] = true;
    }
    }


});
async function  Login() {
    let isAllFieldValid = fieldValidation(isCanBeValidate);

    if (isAllFieldValid) {
        console.log("Insert Campaign");
        let username = document.getElementById("Username");
        let password = document.getElementById("Password");
        let payload = {
            username: username.value,
            password: password.value
        }
        try {
            const response = await fetch("api/Auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload) 
            });

            if (response.ok) {
                const data = await response.json();
                // Token'i localStorage veya sessionStorage gibi bir yerde saklayabilirsiniz
                localStorage.setItem("authToken", data?.token);

                window.location.href = "/Home/Index"

            } else if (response.status === 401) {
                alert("Invalid username or password");
            } else {
                alert("Something went wrong");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("An error occurred while logging in.");
        }
    }
}

