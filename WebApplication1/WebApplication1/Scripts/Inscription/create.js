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
errorDiv.innerHTML = "No se cumplen los requisitos";
errorDiv.style.display = "none";
NewAcquirer.insertAdjacentElement('afterend', errorDiv);


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
    if (NotPercentAlienator.checked) {
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

PercentAcquirer.addEventListener('input', (event) => {
    let sum = 0;
    const percent = parseInt(event.target.value);

    if (isNaN(percent) || percent < 0 || percent > 100) {
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