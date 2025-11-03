function spBulkInsert(docs) {
    var context = getContext();
    var container = context.getCollection();
    var response = context.getResponse();

    var count = 0;

    // Validate input
    if (!docs || !docs.length) {
        response.setBody("No documents to insert.");
        return;
    }

    // Recursive function to insert documents one by one
    var createNext = function () {
        if (count >= docs.length) {
            response.setBody(count + " documents inserted.");
            return;
        }

        var doc = docs[count];

        // Try to create the next document
        var accepted = container.createDocument(container.getSelfLink(), doc, function (err) {
            if (err) throw err;
            count++;
            createNext();
        });

        // If we’re throttled or out of resources, stop for now
        if (!accepted) {
            response.setBody("Partial insert. Inserted " + count + " of " + docs.length + " documents.");
        }
    };

    createNext()
}