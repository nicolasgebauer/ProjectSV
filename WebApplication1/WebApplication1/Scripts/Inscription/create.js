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

NewAcquirer.addEventListener('click', (event) => {
    const NewRowAcquirer = AcquirerTable.insertRow();
    const RutCellAcquirer = NewRowAcquirer.insertCell();
    const PercentCellAcquirer = NewRowAcquirer.insertCell();
    var rut = AcquirerRUT.value;
    var percent = PercentAcquirer.value;

    if (rut.trim() === "") {
        const alertDiv = document.createElement('div');
        alertDiv.className = "alert alert-info";
        alertDiv.innerHTML = "Error: El campo RUT no puede ser vacío";
        document.getElementById('acquirer_title').insertAdjacentElement('beforebegin', alertDiv);
        setTimeout(() => {
            alertDiv.remove();
        }, 3000); // remove after 3 seconds
        event.preventDefault();
        return;
    } else if (NotPercentAcquirer.checked) {
        AllUsers.acquirers_users.push([rut, -1]);
        RutCellAcquirer.innerHTML = rut;
        PercentCellAcquirer.innerHTML = "";
    } else {
        // Check if the sum of percentages is greater than 100
        let sum = parseInt(percent);
        for (let i = 0; i < AllUsers.acquirers_users.length; i++) {
            sum += parseInt(AllUsers.acquirers_users[i][1]);
        }
        if (sum > 100) {
            const alertDiv = document.createElement('div');
            alertDiv.className = "alert alert-info";
            alertDiv.innerHTML = "Error: La suma de porcentajes no puede superar el 100%";
            document.getElementById('acquirer_title').insertAdjacentElement('beforebegin', alertDiv);
            setTimeout(() => {
                alertDiv.remove();
            }, 3000); // remove after 3 seconds
            event.preventDefault();
            return;
        }
        // Add the new acquirer
        AllUsers.acquirers_users.push([rut, percent]);
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