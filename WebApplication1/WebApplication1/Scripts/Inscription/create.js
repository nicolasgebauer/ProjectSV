const allUsers = {
    alienators_users: [],
    acquirers_users: []
}

var InscriptionField = document.querySelector('#date_inscription');
var ActualDate = new Date().toISOString().split('T')[0];
InscriptionField.min = ActualDate;

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
errorDiv.innerHTML = "El Rut debe ser válido";
errorDiv.style.display = "none";

const errorDivAlienator = document.createElement('div');
errorDivAlienator.className = "alert alert-info";
errorDivAlienator.innerHTML = "El Rut debe ser válido";
errorDivAlienator.style.display = "none";

NewAcquirer.insertAdjacentElement('afterend', errorDiv);
NewAlienator.insertAdjacentElement('afterend', errorDivAlienator);
const cneSelect = document.querySelector("#cne_select");
const allAlienatorsDiv = document.querySelector("#all_alienators");
const rutRegex = /^[0-9]+$/;
const comunaValue = document.getElementById("commune_select").value;
const blockValue = document.getElementById("blok_input").value;
const siteValue = document.getElementById("site_input").value;
const pageValue = document.getElementById("page_input").value;
const inscriptionNumberValue = document.getElementById("inscription_number_input").value;




InscriptionField.addEventListener('change', function () {
    var InscriptionDate = new Date(this.value);
    if (InscriptionDate > new Date()) {
        alert("La fecha de inscripción no puede ser una fecha futura.");
        this.value = '';
    }
});

cneSelect.addEventListener("change", () => {      // if the CNE is Regularización de Patrimonio, the Enanejantes form dissapears
    if (cneSelect.value === "Regularización de Patrimonio") {
        allAlienatorsDiv.style.display = "none";
        AlienatorRUT.disabled = true;
        PercentAlienator.disabled = true;
    } else {
        allAlienatorsDiv.style.display = "block";
        PercentAlienator.disabled = false
        AlienatorRUT.disabled = false;
    }
});

NotPercentAlienator.addEventListener('change', () => {   //button of percentage's visibility
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
    if (NotPercentAlienator.checked) {              //Alienators with no defined percentage are added with -1 to distinguish them
        allUsers.alienators_users.push([rut, -1])
        RutCellAlienator.innerHTML = rut;
        PercentCellAlienator.innerHTML = "";
    } else {
        allUsers.alienators_users.push([rut, percent])
        RutCellAlienator.innerHTML = rut;
        PercentCellAlienator.innerHTML = percent;
    }
    AlienatorRUT.value = "";
    PercentAlienator.value = "";
    NotPercentAlienator.checked = false;
});

AcquirerRUT.addEventListener('input', (event) => { //Verifies if the RUT is not repeated and if it only has numbers
    const rut = event.target.value;
    const rutExists = allUsers.acquirers_users.some(user => user[0] === rut);
    const isValidRUT = validateRUT(rut);

    if (!isValidRUT || rutExists) {
        disableButton(NewAcquirer);
        showError(errorDiv);
    } else {
        enableButton(NewAcquirer);
        hideError(errorDiv);
    }
});

AlienatorRUT.addEventListener('input', (event) => {
    const rut = event.target.value;
    const rutExists = allUsers.alienators_users.some(user => user[0] == rut);
    const isValidRUT = validateRUT(rut);
    if (!isValidRUT || rutExists) {
        disableButton(NewAlienator);
        showError(errorDivAlienator);
    } else {
        enableButton(NewAlienator);
        hideError(errorDivAlienator);
    }
});

PercentAcquirer.addEventListener('input', (event) => {
    let sum = 0;
    const percent = parseInt(event.target.value);
    //checks if the sum of percentages is greater than 100
    if (isNaN(percent) || percent < 0 || percent > 100) {
        disableButton(NewAcquirer);
        showError(errorDiv);
        return;
    }

    for (let i = 0; i < allUsers.acquirers_users.length; i++) {
        const percentage = parseInt(allUsers.acquirers_users[i][1]);
        if (percentage !== -1) {
            sum += percentage;
        }
    }
    sum += percent;

    if (sum > 100) {
        disableButton(NewAcquirer);
        showError(errorDiv);
    } else {
        enableButton(NewAcquirer);
        hideError(errorDiv);
    }
});

function disableButton(button) {
    button.disabled = true;
}

function enableButton(button) {
    button.disabled = false;
}

function showError(error) {
    error.style.display = "block";

}

function hideError(error) {
    error.style.display = "none";
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
        allUsers.acquirers_users.push([rut, -1]);
        RutCellAcquirer.innerHTML = rut;
        PercentCellAcquirer.innerHTML = "";
    } else {
        allUsers.acquirers_users.push([rut, percent]);
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
        enableButton(NewAcquirer);
        hideError(errorDiv);
    } else {
        PercentAcquirer.disabled = false;
    }
});


SubmitButtonInscription.addEventListener('click', (event) => {
    for (let i = 0; i < allUsers.acquirers_users.length; i++) {
        const percentage = parseFloat(allUsers.acquirers_users[i][1]);
        if (percentage == -1) {
            allUsers.acquirers_users[i][1] = 0;
        }
    }
    for (let i = 0; i < allUsers.alienators_users.length; i++) {
        const percentage = parseFloat(allUsers.alienators_users[i][1]);
        if (percentage == -1) {
            allUsers.alienators_users[i][1] = 0;
        }
    }

    SubmitButtonInscription.value = JSON.stringify(allUsers)
});

function validateRUT(rut) {
    const rutRegex = /^[0-9]+-[0-9kK]{1}$/;
    if (!rutRegex.test(rut)) {
        return false;
    }
    const [mantisa, dv] = rut.split("-");
    const dvUpperCase = dv.toUpperCase();
    const calculatedDV = calculateDV(mantisa);
    return dvUpperCase === calculatedDV;
}

function calculateDV(mantisa) {
    const rutDigits = mantisa.split("").reverse();
    let factor = 2;
    let sum = 0;
    for (let i = 0; i < rutDigits.length; i++) {
        sum += parseInt(rutDigits[i]) * factor;
        factor = factor === 7 ? 2 : factor + 1;
    }
    const remainder = sum % 11;
    const calculatedDV = 11 - remainder;
    if (calculatedDV === 11) {
        return "0";
    } else if (calculatedDV === 10) {
        return "K";
    } else {
        return calculatedDV.toString();
    }
}