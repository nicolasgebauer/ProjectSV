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


NotPercentAlienator.addEventListener('change', () => {
    if (NotPercentAlienator.checked) {
        PercentAlienator.disabled = true;
    } else {
        PercentAlienator.disabled = false;
    }
});

NotPercentAcquirer.addEventListener('change', () => {
    if (NotPercentAcquirer.checked) {
        PercentAcquirer.disabled = true;
    } else {
        PercentAcquirer.disabled = false;
    }
});

NewAlienator.addEventListener('click', () => {
    console.log("aca");
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

NewAcquirer.addEventListener('click', () => {
    const NewRowAcquirer = AcquirerTable.insertRow()
    const RutCellAcquirer = NewRowAcquirer.insertCell()
    const PercentCellAcquirer = NewRowAcquirer.insertCell()
    var rut = AcquirerRUT.value
    var percent = PercentAcquirer.value
    if (NotPercentAcquirer.checked) {
        AllUsers.acquirers_users.push([rut, -1])
        RutCellAcquirer.innerHTML = rut;
        PercentCellAcquirer.innerHTML = "";
    } else {
        AllUsers.acquirers_users.push([rut, percent])
        RutCellAcquirer.innerHTML = rut;
        PercentCellAcquirer.innerHTML = percent;
    }
    AcquirerRUT.value = "";
    PercentAcquirer.value = "";
    NotPercentAcquirer.checked = false;
});

SubmitButtonInscription.addEventListener('click', () => {
    SubmitButtonInscription.value = JSON.stringify(AllUsers)
});