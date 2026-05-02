FROM rust:slim AS builder

RUN rustup target add x86_64-unknown-linux-musl \
    && apt-get update -q \
    && apt-get install -y --no-install-recommends musl-tools \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Build with a stub file to compile dependencies
# This caches the dependencies layer for faster rebuilds if Cargo.toml/lock isn't modified
COPY Cargo.toml Cargo.lock ./
RUN mkdir src \
    && echo 'fn main() {}' > src/main.rs \
    && cargo build --release --target x86_64-unknown-linux-musl \
    && rm -rf src

# touch makes sure build properly builds rather than thinking main.rs is already compiled
COPY src ./src
RUN touch src/main.rs \
    && cargo build --release --target x86_64-unknown-linux-musl

FROM scratch

WORKDIR /app

COPY --from=builder /build/target/x86_64-unknown-linux-musl/release/hydra /app/hydra

ENTRYPOINT ["/app/hydra"]
