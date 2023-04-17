const AllUsers = {
    alienators_users: [],
    acquirers_users: []
}


const NewAlienator = document.querySelector("#new_alienator");
const AlienatorRUT = document.querySelector("#rut_alienator");

const PercentAlienator = document.querySelector("#percent_alienator");
const NotPercentAlienator = document.querySelector("#not_percent_alienator");
const AlienatorTable = document.querySelector("#alienators").getElementsByTagName("tbody")[0];

const NewAcquirer = document.querySelector("#new_acquirer");
const AcquirerRUT = document.querySelector("#rut_acquirer");
const PercentAcquirer = document.querySelector("#percent_acquirer");
const NotPercentAcquirer = document.querySelector("#not_percent_acquirer");
const AcquirerTable = document.querySelector("#acquirers").getElementsByTagName("tbody")[0];

const SubmitButtonInscription = document.querySelector("#submit_button");
const errorDiv = document.createElement('div');
errorDiv.className = "alert alert-info";
errorDiv.innerHTML = "Comprobar que el RUT no haya sido ingresado, que no tenga puntos ni guión y que la suma de porcentajes sea menor a 100";
errorDiv.style.display = "none";
NewAcquirer.insertAdjacentElement('afterend', errorDiv);
const cneSelect = document.querySelector("#cne_select");
const allAlienatorsDiv = document.querySelector("#all_alienators");
const rutRegex = /^[0-9]+$/;


cneSelect.addEventListener("change", () => {
    if (cneSelect.value === "Regularización de Patrimonio") {
        allAlienatorsDiv.style.display = "none";
    } else {
        allAlienatorsDiv.style.display = "block";
    }
});



NotPercentAlienator.addEventListener('change', () => {
    if (NotPercentAlienator.checked) {
        PercentAlienator.disabled = true;
    } else {
        PercentAlienator.disabled = false;
    }
});




NewAlienator.addEventListener('click', () => {          
    const NewRowAlienator = AlienatorTable.insertRow()
    const RutCellAlienator = NewRowAlienator.insertCell()
    const PercentCellAlienator = NewRowAlienator.insertCell()
    var rut = AlienatorRUT.value
    var percent = PercentAlienator.value
    if (NotPercentAlienator.checked) {              //Cases where 
        AllUsers.alienators_users.push([rut, -1])
        RutCellAlienator.innerHTML = rut;
        PercentCellAlienator.innerHTML = "";
    } else {
        AllUsers.alienators_users.push([rut, percent])
        RutCellAlienator.innerHTML = rut;
        PercentCellAlienator.innerHTML = percent;
    }
    AlienatorRUT.value = "";
    PercentAlienator.value = "";
    NotPercentAlienator.checked = false;
});

AcquirerRUT.addEventListener('input', (event) => { //Verifies if the RUT is not repeated and if it only has numbers
    const rut = event.target.value;
    const rutExists = AllUsers.acquirers_users.some(user => user[0] === rut);
    if (!rutRegex.test(rut)) {
        disableButton();
        showError();
    } else if (rutExists) {
        disableButton();
        showError();
    } else {
        enableButton();
        hideError();
    }
});

PercentAcquirer.addEventListener('input', (event) => {  
    let sum = 0;
    const percent = parseInt(event.target.value);

    if (isNaN(percent) || percent < 0 || percent > 100) { //checks if the sum of percentages is greater than 100
        disableButton();
        showError();
        return;
    }

    for (let i = 0; i < AllUsers.acquirers_users.length; i++) {
        const percentage = parseInt(AllUsers.acquirers_users[i][1]);
        if (percentage !== -1) {
            sum += percentage;
        }
    }
    sum += percent;

    if (sum > 100) {
        disableButton();
        showError();
    } else {
        enableButton();
        hideError();
    }
});



function disableButton() {
    NewAcquirer.disabled = true;
}

function enableButton() {
    NewAcquirer.disabled = false;
}

function showError() {
    errorDiv.style.display = "block";

}



function hideError() {
    errorDiv.style.display = "none";
}



function findRUT(element, rut) {
    return element[0] === rut;
}

NewAcquirer.addEventListener('click', (event) => {
    const NewRowAcquirer = AcquirerTable.insertRow();
    const RutCellAcquirer = NewRowAcquirer.insertCell();
    const PercentCellAcquirer = NewRowAcquirer.insertCell();
    var rut = AcquirerRUT.value;
    var percent = PercentAcquirer.value;

    if (NotPercentAcquirer.checked) {
        AllUsers.acquirers_users.push([rut, -1]);
        RutCellAcquirer.innerHTML = rut;
        PercentCellAcquirer.innerHTML = "";
    } else {
        AllUsers.acquirers_users.push([rut, percent]);
        RutCellAcquirer.innerHTML = rut;
        PercentCellAcquirer.innerHTML = percent;
    }
    AcquirerRUT.value = "";
    PercentAcquirer.value = "";
    NotPercentAcquirer.checked = false;
});

NotPercentAcquirer.addEventListener('change', () => {
    if (NotPercentAcquirer.checked) {
        PercentAcquirer.disabled = true;
        PercentAcquirer.value = "";
        enableButton();
        hideError();
    } else {
        PercentAcquirer.disabled = false;
    }
});


SubmitButtonInscription.addEventListener('click', () => {
    SubmitButtonInscription.value = JSON.stringify(AllUsers)
});
